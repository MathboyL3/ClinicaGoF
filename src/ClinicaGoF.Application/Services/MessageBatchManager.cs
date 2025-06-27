using ClinicaGoF.Application.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaGoF.Application.Services
{
    public class MessageBatchManager : IMessageBatchManager
    {
        private readonly INotificationService _notificationService;
        private readonly ConcurrentQueue<Message> _messageQueue = new();
        private readonly Timer _batchTimer;
        private readonly int _batchSize;
        private readonly int _batchIntervalMs;
        private readonly SemaphoreSlim _sendingSemaphore = new(1, 1);
        private bool _disposed;

        public MessageBatchManager(
            INotificationService notificationService,
            int batchSize = 10,
            int batchIntervalMs = 30000) // Default: 30 seconds
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _batchSize = batchSize > 0 ? batchSize : throw new ArgumentException("Batch size must be greater than zero", nameof(batchSize));
            _batchIntervalMs = batchIntervalMs > 0 ? batchIntervalMs : throw new ArgumentException("Batch interval must be greater than zero", nameof(batchIntervalMs));

            // Set up timer to process messages at regular intervals
            _batchTimer = new Timer(ProcessBatchCallback, null, _batchIntervalMs, _batchIntervalMs);
        }

        public void QueueMessage(string recipient, string subject, string content, NotificationType type)
        {
            if (string.IsNullOrEmpty(recipient))
                throw new ArgumentException("Recipient cannot be null or empty", nameof(recipient));

            _messageQueue.Enqueue(new Message(recipient, subject, content, type));

            // If we've hit the batch size, trigger sending immediately
            if (_messageQueue.Count >= _batchSize)
            {
                TriggerBatchProcessing();
            }
        }

        public Task QueueMessageAsync(string recipient, string subject, string content, NotificationType type)
        {
            QueueMessage(recipient, subject, content, type);
            return Task.CompletedTask;
        }

        public void TriggerBatchProcessing()
        {
            // Signal the timer to process immediately
            _batchTimer.Change(0, _batchIntervalMs);
        }

        private void ProcessBatchCallback(object? state)
        {
            // Fire and forget - the method handles its own exceptions
            _ = ProcessBatchAsync();
        }

        private async Task ProcessBatchAsync()
        {
            // Ensure only one batch process runs at a time
            if (!await _sendingSemaphore.WaitAsync(0))
                return;

            try
            {
                List<Message> messagesToSend = new();

                // Dequeue up to batchSize messages
                for (int i = 0; i < _batchSize; i++)
                {
                    if (!_messageQueue.TryDequeue(out var message))
                        break;

                    messagesToSend.Add(message);
                }

                if (messagesToSend.Count == 0)
                    return;

                // Send all messages in parallel
                var sendTasks = messagesToSend.Select(msg =>
                    _notificationService.SendNotificationAsync(
                        msg.Recipient,
                        msg.Subject,
                        msg.Content,
                        msg.Type));

                await Task.WhenAll(sendTasks);
            }
            catch (Exception ex)
            {
                // Log the exception - in a real implementation, use a proper logger
                Console.WriteLine($"Error processing message batch: {ex}");
            }
            finally
            {
                _sendingSemaphore.Release();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Process any remaining messages
                ProcessBatchAsync().Wait();

                _batchTimer.Dispose();
                _sendingSemaphore.Dispose();
            }

            _disposed = true;
        }

        // Message record to store in the queue
        private record Message(string Recipient, string Subject, string Content, NotificationType Type);
    }
}
