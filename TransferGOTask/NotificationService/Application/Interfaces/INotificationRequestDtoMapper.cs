namespace TransferGOTask.NotificationService.Application.Interfaces;

using DTOs;
using Domain.Entities;

public interface INotificationRequestDtoMapper
{
    Notification Map(NotificationRequestDto apiRequest);
}