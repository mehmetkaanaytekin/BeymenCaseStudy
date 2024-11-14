#nullable disable
namespace StockService.Domain.Entities
{
    public class StockMovement
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid OrderId { get; set; }
        public DateTime MovementDate { get; set; }
        public int Quantity { get; set; }
        public int MovementType { get; set; } // e.g., "IN" for stock in, "OUT" for stock out
    }
}