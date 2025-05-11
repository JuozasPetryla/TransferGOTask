namespace TransferGOTask.NotificationService.Application.DTOs;

using Domain.Enums;
using Domain.ValueObjects;

public class NotificationRequestDto
{
    public ChannelType Channel { get; set; }
    public RecipientInfo Recipient { get; set; }
    public MessageContent Message { get; set; }
}