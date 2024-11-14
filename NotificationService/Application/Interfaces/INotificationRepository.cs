using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces
{
    public interface INotificationRepository
    {
        void SaveNotificationRecord(Notification notification);
    }
}
