using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StockService.Application.Services;
using StockService.Domain.Dtos;
using System.Text;

namespace StockService.Infrastructure.Services
{
    public class StockMQService
    {
        private readonly IChannel _channel;
        private readonly IServiceProvider _serviceProvider;
        private const string OrderQueue = "orderQueue";
        private const string ProcessedOrderQueue = "processedOrderQueue";
        private const string DeadLetterQueue = "deadLetterQueue";
        private const int MaxRetryCount = 3;

        public StockMQService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            var connection = factory.CreateConnectionAsync().Result;
            _channel = connection.CreateChannelAsync().Result;
            _channel.QueueDeclareAsync(queue: DeadLetterQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclareAsync(queue: OrderQueue, durable: false, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", DeadLetterQueue }
            });
            _channel.QueueDeclareAsync(queue: ProcessedOrderQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartListening(Action<string> onMessageReceived)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(message);

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var stockHandler = scope.ServiceProvider.GetRequiredService<StockHandler>();
                        if (stockHandler != null && orderMessage?.Order != null)
                        {
                            stockHandler.ProcessOrderMessage(orderMessage.Order);
                            orderMessage.State = "ProcessedByStockService";
                            onMessageReceived(JsonConvert.SerializeObject(orderMessage));

                            var updatedMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(orderMessage));
                            _channel.BasicPublishAsync(exchange: "", routingKey: ProcessedOrderQueue, body: updatedMessage);
                            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    await HandleProcessingFailure(ea);
                }
            };
            _channel.BasicConsumeAsync(queue: OrderQueue, autoAck: false, consumer: consumer);
            Console.WriteLine("Started listening to the queue.");
        }

        private async Task HandleProcessingFailure(BasicDeliverEventArgs ea)
        {
            var retryCount = ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.ContainsKey("x-retry-count")
                ? (int?)ea.BasicProperties.Headers["x-retry-count"] ?? 0 : 0;

            if (retryCount < MaxRetryCount)
            {
                var properties = new BasicProperties();
                properties.Headers = new Dictionary<string, object?> { { "x-retry-count", retryCount + 1 } };

                await _channel.BasicPublishAsync(exchange: "", routingKey: OrderQueue, true, basicProperties: properties, body: ea.Body);
            }
            else
            {
                await _channel.BasicPublishAsync(exchange: "", routingKey: DeadLetterQueue, body: ea.Body);
            }

            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
}
