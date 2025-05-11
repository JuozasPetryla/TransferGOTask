namespace TransferGOTask.NotificationService.Application.Exceptions;

public class NotificationNotFoundException : Exception
{
    public NotificationNotFoundException(Guid notificationId)
        : base($"Notification with ID '{notificationId}' was not found.")
    {
    }
}