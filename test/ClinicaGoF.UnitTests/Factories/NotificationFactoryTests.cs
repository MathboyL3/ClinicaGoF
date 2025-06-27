using ClinicaGoF.Application.Services;
using Shouldly;

namespace ClinicaGoF.UnitTests.Factories
{
    public class NotificationFactoryTests
    {
        private readonly NotificationFactory _factory;

        public NotificationFactoryTests()
        {
            _factory = new NotificationFactory();
        }

        [Fact]
        public void CreateNotification_ShouldReturnEmailNotification_WhenTypeIsEmail()
        {
            // Act
            var notification = _factory.CreateNotification(NotificationType.Email);

            // Assert
            notification.ShouldNotBeNull();
            notification.ShouldBeOfType<EmailNotification>();
        }

        [Fact]
        public void CreateNotification_ShouldReturnSMSNotification_WhenTypeIsSMS()
        {
            // Act
            var notification = _factory.CreateNotification(NotificationType.SMS);

            // Assert
            notification.ShouldNotBeNull();
            notification.ShouldBeOfType<SMSNotification>();
        }

        [Fact]
        public void CreateNotification_ShouldThrowArgumentException_WhenTypeIsInvalid()
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() => 
                _factory.CreateNotification((NotificationType)999))
                .ParamName.ShouldBe("type");
        }

        [Fact]
        public void CreateNotification_ReturnedObjects_ShouldImplementINotificationInterface()
        {
            // Act
            var emailNotification = _factory.CreateNotification(NotificationType.Email);
            var smsNotification = _factory.CreateNotification(NotificationType.SMS);

            // Assert
            emailNotification.ShouldBeAssignableTo<INotification>();
            smsNotification.ShouldBeAssignableTo<INotification>();
        }
    }
}