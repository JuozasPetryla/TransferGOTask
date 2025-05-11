namespace TransferGOTask.NotificationService.Application.Interfaces;

using DTOs;
using Domain.Enums;

public interface INotificationService
{
    Task<NotificationResponseDto> SendNotificationAsync(
        NotificationRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<NotificationStatus> GetStatusAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default);
}
