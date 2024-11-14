#nullable disable
using OrderService.Domain.Entities;

namespace OrderService.Domain.Dtos
{
    public class OrderMessage
    {
        public Order Order { get; set; }
        public string State { get; set; }
    }
}