using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace OrderService.Infrastructure.Services
{
    public class OrderMQService
    {
        private readonly ConnectionFactory _factory;
        private const string OrderQueue = "orderQueue";
        private const string DeadLetterQueue = "deadLetterQueue";

        public OrderMQService()
        {
            _factory = new ConnectionFactory { HostName = "rabbitmq" };
        }

        public async Task SendMessageAsync(string message)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            channel.QueueDeclareAsync(queue: DeadLetterQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclareAsync(queue: OrderQueue, durable: false, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", DeadLetterQueue }
            });

            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: OrderQueue, body: body);

        }
    }
}