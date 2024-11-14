#nullable disable
using OrderService.Domain.Enum;

namespace OrderService.Domain.Dtos
{
    public class OrderDto
    {
        public Guid CustomerId { get; set; }
        public string OrderNumber { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string Address { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
    }

    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public int LineNumber { get; set; }
        public string Barcode { get; set; }
        public decimal UnitPrice { get; set; }
    }
}