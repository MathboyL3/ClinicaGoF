using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaGoF.Application.Services.Interfaces
{
    public interface IMessageBatchManager : IDisposable
    {
        void QueueMessage(string recipient, string subject, string content, NotificationType type);
        Task QueueMessageAsync(string recipient, string subject, string content, NotificationType type);
        void TriggerBatchProcessing();
    }
}
