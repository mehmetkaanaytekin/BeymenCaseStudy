using StockService.Application.Interfaces;
using StockService.Domain.Dtos;
using StockService.Domain.Enum;

namespace StockService.Application.Services
{
    // Services/StockService.cs
    public class StockHandler
    {
        private readonly IStockRepository _stockRepository;

        public StockHandler(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public void ProcessOrderMessage(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                var stockMovement = _stockRepository.GetStockMovements(order.Id);
                if (stockMovement != null)
                {
                    stockMovement.Quantity = item.Quantity;
                    stockMovement.MovementType = (int)MovementType.OUT;

                    _stockRepository.UpdateStockMovement(stockMovement);
                    return;
                }

                stockMovement = new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    OrderId = order.Id,
                    MovementDate = DateTime.Now,
                    Quantity = item.Quantity,
                    MovementType = (int)MovementType.OUT
                };

                _stockRepository.CreateStockMovement(stockMovement);
            }
        }
    }
}