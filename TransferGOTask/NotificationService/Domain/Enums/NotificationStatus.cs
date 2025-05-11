namespace TransferGOTask.NotificationService.Domain.Enums;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationStatus
{
    Pending = 0,
    InProgress = 1,
    Sent = 2,
    Failed = 3,
    Retrying = 4
}
