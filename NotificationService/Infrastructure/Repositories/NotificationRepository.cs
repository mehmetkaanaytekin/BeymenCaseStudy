using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDBContext _context;
        public NotificationRepository(NotificationDBContext context)
        {
            _context = context;
        }
        public void SaveNotificationRecord(Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }
    }
}
