namespace TransferGOTask.NotificationService.Infrastructure.Mappers;

using Application.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Domain.ValueObjects;

public class NotificationRequestDtoMapper : INotificationRequestDtoMapper
{
    public Notification Map(NotificationRequestDto request)
    {
        var recipient = new RecipientInfo(
            phoneNumber: request.Recipient.PhoneNumber,
            email:       request.Recipient.Email
        );

        var message = new MessageContent(
            subject: request.Message.Subject,
            body:    request.Message.Body
        );
        
        return new Notification(
            channel:   request.Channel,
            recipient: recipient,
            message:   message
        );
    }
}