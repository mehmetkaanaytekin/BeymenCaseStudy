#nullable disable
namespace OrderService.Domain.Entities
{
    public class OrderItem
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int LineNumber { get; set; }
        public string Barcode { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}