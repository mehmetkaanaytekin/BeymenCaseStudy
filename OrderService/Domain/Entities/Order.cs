#nullable disable
using OrderService.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; } = Guid.NewGuid();
        public string OrderNumber { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string Address { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public Order()
        {
            TotalAmount = OrderItems?.Sum(x => x.UnitPrice * x.Quantity) ?? 0;
        }
    }
}
