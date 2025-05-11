namespace TransferGOTask.NotificationService.Application.Exceptions;

public class NotificationFailedException : Exception
{
    public NotificationFailedException() { }
    public NotificationFailedException(string message) : base(message) { }
    public NotificationFailedException(string message, Exception inner) : base(message, inner) { }
}