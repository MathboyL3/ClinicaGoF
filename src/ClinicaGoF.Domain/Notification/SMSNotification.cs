public class SMSNotification : INotification
{
    public Task SendAsync(string recipient, string subject, string message)
    {
        // Fake implementation for sending SMS
        Console.WriteLine($"SMS sent to {recipient}");
        Console.WriteLine($"Message: {subject} - {message}");

        return Task.CompletedTask;
    }
}