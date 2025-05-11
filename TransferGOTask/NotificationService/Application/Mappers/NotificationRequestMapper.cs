namespace TransferGOTask.NotificationService.Application.Mappers;

using Api.Models;
using DTOs;
using Interfaces;
using Domain.ValueObjects;

public class NotificationRequestMapper : INotificationRequestMapper
{
    public NotificationRequestDto Map(NotificationRequest r) =>
        new NotificationRequestDto {
            Channel   = r.Channel,
            Recipient = new RecipientInfo(r.Recipient.PhoneNumber, r.Recipient.Email, r.Recipient.DeviceToken),
            Message   = new MessageContent(r.Message.Subject, r.Message.Body)
        };
}