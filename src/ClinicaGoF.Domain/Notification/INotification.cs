﻿public interface INotification
{
    Task SendAsync(string recipient, string subject, string message);
}