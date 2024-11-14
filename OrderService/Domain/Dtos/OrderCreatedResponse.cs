#nullable disable
using OrderService.Domain.Enum;

namespace OrderService.Domain.Dtos
{
    public class OrderCreatedResponse
    {
        public string OrderId { get; set; }
        public bool OrderReceived { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}