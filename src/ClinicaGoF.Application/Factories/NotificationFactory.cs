using ClinicaGoF.Application.Services;

public class NotificationFactory : INotificationFactory
{
    public INotification CreateNotification(NotificationType type)
    {
        return type switch
        {
            NotificationType.Email => new EmailNotification(),
            NotificationType.SMS => new SMSNotification(),
            _ => throw new ArgumentException($"Notification type {type} not supported", nameof(type))
        };
    }
}
