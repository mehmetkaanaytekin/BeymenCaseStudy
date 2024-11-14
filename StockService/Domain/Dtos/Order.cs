namespace StockService.Domain.Dtos
{
    // Models/Order.cs
    public class Order
    {
        public Guid Id { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }

    // Models/OrderItem.cs
    public class OrderItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

}
