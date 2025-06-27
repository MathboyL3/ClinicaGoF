using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaGoF.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string recipient, string subject, string message, NotificationType type);
    }
}
