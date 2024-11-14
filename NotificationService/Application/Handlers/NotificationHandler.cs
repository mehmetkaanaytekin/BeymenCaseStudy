using NotificationService.Domain.Entities;
using System.Net.Mail;
using System.Net;
using NotificationService.Domain.Dtos;
using NotificationService.Application.Interfaces;

namespace NotificationService.Application.Handlers
{
    public class NotificationHandler
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        public void ProcessOrderMessage(Order order)
        {
            var emailMessage = $"Your order {order.OrderNumber} has been created.";
            var smsMessage = $"Order {order.OrderNumber} created.";

            SendEmail(order.CustomerEmail, "Order Created", emailMessage);
            SendSms(order.CustomerPhone, smsMessage);

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                CustomerEmail = order.CustomerEmail,
                CustomerPhone = order.CustomerPhone,
                Message = emailMessage,
                SentDate = DateTime.UtcNow
            };

            _notificationRepository.SaveNotificationRecord(notification);
        }

        public void SendEmail(string toEmail, string subject, string message)
        {
            var fromEmail = "your-email@example.com";
            var fromPassword = "your-email-password";

            var smtpClient = new SmtpClient("smtp.example.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
        }

        public void SendSms(string toPhoneNumber, string message)
        {
            Console.WriteLine($"Sending SMS to {toPhoneNumber}: {message}");
        }
    }

}
