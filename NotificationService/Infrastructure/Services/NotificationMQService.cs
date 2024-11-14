using Newtonsoft.Json;
using NotificationService.Application.Handlers;
using NotificationService.Domain;
using NotificationService.Domain.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace NotificationService.Infrastructure.Services
{
    public class NotificationMQService
    {
        private readonly IChannel _channel;
        private readonly NotificationHandler _notificationHandler;
        private const string ProcessedOrderQueue = "processedOrderQueue";
        private const string DeadLetterQueue = "deadLetterQueue";
        private const int MaxRetryCount = 3;

        public NotificationMQService(NotificationHandler notificationHandler)
        {
            _notificationHandler = notificationHandler;

            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            var connection = factory.CreateConnectionAsync().Result;
            _channel = connection.CreateChannelAsync().Result;

            _channel.QueueDeclareAsync(queue: ProcessedOrderQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclareAsync(queue: DeadLetterQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartListening()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(message);
                var retryCount = ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.ContainsKey("x-retry-count")
                    ? (int?)ea.BasicProperties.Headers["x-retry-count"] ?? 0 : 0;

                try
                {
                    if (orderMessage?.State == "ProcessedByStockService")
                    {
                        var notification = new Notification
                        {
                            Id = Guid.NewGuid(),
                            CustomerEmail = orderMessage.Order.CustomerEmail,
                            CustomerPhone = orderMessage.Order.CustomerPhone,
                            Message = "Your order has been processed by the stock service.",
                            SentDate = DateTime.UtcNow
                        };

                        _notificationHandler.SendEmail(notification.CustomerEmail, "Order Processed", notification.Message);
                        _notificationHandler.SendSms(notification.CustomerPhone, notification.Message);
                    }

                    if (orderMessage != null)
                    {
                        _notificationHandler.ProcessOrderMessage(orderMessage.Order);
                        orderMessage.State = "ProcessedByNotificationService";
                    }

                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception)
                {
                    await HandleProcessingFailure(ea, body, retryCount);
                }
            };
            _channel.BasicConsumeAsync(queue: ProcessedOrderQueue, autoAck: false, consumer: consumer);
        }

        private async Task HandleProcessingFailure(BasicDeliverEventArgs ea, byte[] body, int retryCount)
        {
            if (retryCount < MaxRetryCount)
            {
                var properties = new BasicProperties();
                properties.Headers = new Dictionary<string, object?> { { "x-retry-count", retryCount + 1 } };

                await _channel.BasicPublishAsync(exchange: "", routingKey: ProcessedOrderQueue, true, basicProperties: properties, body: body);
            }
            else
            {
                await _channel.BasicPublishAsync(exchange: "", routingKey: DeadLetterQueue, body: ea.Body);
            }

            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
}