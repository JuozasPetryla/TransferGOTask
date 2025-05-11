namespace TransferGOTask.NotificationService.Domain.Interfaces;

using Entities;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<Notification> GetByIdAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetPendingAsync(CancellationToken cancellationToken = default);
}