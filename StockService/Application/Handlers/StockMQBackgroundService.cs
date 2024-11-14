using StockService.Infrastructure.Services;

namespace StockService.Application.Handlers
{
    public class StockMQBackgroundService : BackgroundService
    {
        private readonly StockMQService _stockMQService;

        public StockMQBackgroundService(StockMQService stockMQService)
        {
            _stockMQService = stockMQService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _stockMQService.StartListening(message =>
            {
                Console.WriteLine($"Received message: {message}");
            });

            return Task.CompletedTask;
        }
    }
}