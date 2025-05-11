namespace TransferGOTask.NotificationService.Domain.Enums;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChannelType
{
    Sms = 1,
    Email = 2
}
