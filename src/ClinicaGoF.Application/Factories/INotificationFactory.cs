using ClinicaGoF.Application.Services;

public interface INotificationFactory
{
    INotification CreateNotification(NotificationType type);
}
