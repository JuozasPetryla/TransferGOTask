namespace TransferGOTask.NotificationService.Application.Interfaces;

using Api.Models;
using DTOs;

public interface INotificationRequestMapper
{
    NotificationRequestDto Map(NotificationRequest apiRequest);
}