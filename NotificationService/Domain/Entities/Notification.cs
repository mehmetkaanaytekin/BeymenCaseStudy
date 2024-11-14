#nullable disable
namespace NotificationService.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
    }
}