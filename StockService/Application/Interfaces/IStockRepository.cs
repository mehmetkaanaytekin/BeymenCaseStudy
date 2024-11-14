using StockService.Domain.Entities;

namespace StockService.Application.Interfaces
{
    public interface IStockRepository
    {
        Task CreateStockMovement(StockMovement stockMovement);
        Task UpdateStockMovement(StockMovement stockMovement);
        StockMovement GetStockMovements(Guid OrderId);
    }
}
