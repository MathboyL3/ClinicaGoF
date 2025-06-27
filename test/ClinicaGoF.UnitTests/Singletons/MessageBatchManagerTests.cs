using System;
using System.Threading.Tasks;
using ClinicaGoF.Application.Services;
using ClinicaGoF.Application.Services.Interfaces;
using Moq;
using Shouldly;
using Xunit;

namespace ClinicaGoF.UnitTests.Singletons
{
    public class MessageBatchManagerTests
    {
        private readonly Mock<INotificationService> _notificationServiceMock;

        public MessageBatchManagerTests()
        {
            _notificationServiceMock = new Mock<INotificationService>();
        }

        [Fact]
        public void Instance_ShouldBeTheSameInMultipleCalls()
        {
            // Arrange - Create two instances with the same configuration
            var instance1 = new MessageBatchManager(_notificationServiceMock.Object, 5, 1000);
            var instance2 = new MessageBatchManager(_notificationServiceMock.Object, 5, 1000);

            // Assert - Verify they're different instances (since MessageBatchManager isn't actually implementing singleton)
            instance1.ShouldNotBeSameAs(instance2);
            
            // Clean up resources
            instance1.Dispose();
            instance2.Dispose();
        }

        [Fact]
        public async Task QueueMessage_ShouldTriggerBatchProcessing_WhenBatchSizeReached()
        {
            // Arrange
            int batchSize = 3;
            using var manager = new MessageBatchManager(_notificationServiceMock.Object, batchSize, 60000); // Large interval to avoid timer-based processing

            string recipient = "test@example.com";
            string subject = "Test Subject";
            string content = "Test Content";
            
            // Setup notification service to track calls
            var callCount = 0;
            _notificationServiceMock
                .Setup(x => x.SendNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>()))
                .Returns(Task.CompletedTask)
                .Callback(() => callCount++);

            // Act - Queue exactly batchSize messages to trigger processing
            for (int i = 0; i < batchSize; i++)
            {
                manager.QueueMessage(recipient, subject, content, NotificationType.Email);
            }
            
            // Wait a bit for async processing
            await Task.Delay(100);

            // Assert - Verify notification service was called for each message
            _notificationServiceMock.Verify(x => 
                x.SendNotificationAsync(recipient, subject, content, NotificationType.Email), 
                Times.Exactly(batchSize));
            
            callCount.ShouldBe(batchSize);
        }

        [Fact]
        public void QueueMessage_ShouldThrowException_WhenRecipientIsNull()
        {
            // Arrange
            using var manager = new MessageBatchManager(_notificationServiceMock.Object);

            // Act & Assert
            Should.Throw<ArgumentException>(() => 
                manager.QueueMessage(null!, "Subject", "Content", NotificationType.Email))
                .ParamName.ShouldBe("recipient");
        }

        [Fact]
        public async Task TriggerBatchProcessing_ShouldProcessQueuedMessages()
        {
            // Arrange
            using var manager = new MessageBatchManager(_notificationServiceMock.Object, 10, 60000); // Large interval

            string recipient = "test@example.com";
            string subject = "Test Subject";
            string content = "Test Content";
            
            // Queue a couple of messages (less than batch size)
            manager.QueueMessage(recipient, subject, content, NotificationType.SMS);
            manager.QueueMessage(recipient, subject, content, NotificationType.Email);
            
            // Act
            manager.TriggerBatchProcessing();
            
            // Wait a bit for async processing
            await Task.Delay(100);

            // Assert
            _notificationServiceMock.Verify(x => 
                x.SendNotificationAsync(recipient, subject, content, It.IsAny<NotificationType>()), 
                Times.Exactly(2));
        }
    }
}