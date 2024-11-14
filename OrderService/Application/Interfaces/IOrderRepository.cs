using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Guid> CreateOrderAsync(Order order);
    }
}