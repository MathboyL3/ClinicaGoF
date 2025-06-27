using ClinicaGoF.Application.Services.Interfaces;

namespace ClinicaGoF.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationFactory _notificationFactory;

        public NotificationService(INotificationFactory notificationFactory)
        {
            _notificationFactory = notificationFactory ?? throw new ArgumentNullException(nameof(notificationFactory));
        }

        public async Task SendNotificationAsync(string recipient, string subject, string message, NotificationType type)
        {
            var notification = _notificationFactory.CreateNotification(type);
            await notification.SendAsync(recipient, subject, message);
        }
    }
}
