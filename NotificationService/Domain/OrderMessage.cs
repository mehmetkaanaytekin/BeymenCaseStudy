#nullable disable
using NotificationService.Domain.Dtos;

namespace NotificationService.Domain
{
    public class OrderMessage
    {
        public Order Order { get; set; }
        public string State { get; set; }
    }
}
