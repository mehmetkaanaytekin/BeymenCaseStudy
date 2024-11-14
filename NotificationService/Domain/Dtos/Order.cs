#nullable disable
namespace NotificationService.Domain.Dtos
{
    public class Order
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

}
