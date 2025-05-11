namespace TransferGOTask.NotificationService.Api.Models;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Enums;

public class NotificationRequest
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChannelType Channel { get; set; }
    
    [Required]
    public RecipientDto Recipient { get; set; }
    
    [Required]
    public MessageDto Message { get; set; }
}

public class RecipientDto
{
    [Phone]
    public string PhoneNumber { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }
    
    public string DeviceToken { get; set; }
}

public class MessageDto
{
    public string Subject { get; set; }
    
    [Required]
    public string Body { get; set; }
}
