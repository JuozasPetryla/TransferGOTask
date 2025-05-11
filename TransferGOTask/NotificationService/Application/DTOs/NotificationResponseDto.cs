using TransferGOTask.NotificationService.Domain.Enums;

namespace TransferGOTask.NotificationService.Application.DTOs;

public class NotificationResponseDto
{
    public Guid NotificationId { get; set; }
    public NotificationStatus Status { get; set; }
    
    public string? DeliveredBy { get; set; }
}