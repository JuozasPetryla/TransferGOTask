namespace TransferGOTask.NotificationService.Domain.Interfaces;

using Entities;
using Enums;

public interface INotificationProvider
{
    string Key { get; }
    ChannelType Channel { get; }
    Task SendAsync(Notification notification, CancellationToken cancellationToken = default);
}