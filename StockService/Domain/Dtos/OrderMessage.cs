namespace StockService.Domain.Dtos
{
    public class OrderMessage
    {
        public Order Order { get; set; }
        public string State { get; set; }
    }
}
