using StockService.Application.Interfaces;
using StockService.Domain.Entities;
using StockService.Infrastructure.Data;

namespace StockService.Infrastructure.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly StockDBContext _context;

        public StockRepository(StockDBContext stockDBContext)
        {
            _context = stockDBContext;
        }
        public async Task CreateStockMovement(StockMovement stockMovement)
        {
            await _context.AddAsync(stockMovement);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStockMovement(StockMovement stockMovement)
        {
            _context.Update(stockMovement);
            await _context.SaveChangesAsync();
        }
        public StockMovement GetStockMovements(Guid OrderId)
        {
            StockMovement stockMovement = _context.StockMovements.FirstOrDefault(x => x.OrderId == OrderId) ?? new StockMovement();
            return stockMovement;
        }
    }
}