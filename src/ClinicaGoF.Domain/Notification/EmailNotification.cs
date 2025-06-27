public class EmailNotification : INotification
{
    public Task SendAsync(string recipient, string subject, string message)
    {
        // Fake implementation for sending emails
        Console.WriteLine($"Email sent to {recipient}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");

        return Task.CompletedTask;
    }
}
