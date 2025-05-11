namespace TransferGOTask.NotificationService.Infrastructure.Configuration;

public class TwilioSettings
{
    public string AccountSid          { get; set; }
    public string AuthToken           { get; set; }
    public string FromNumber          { get; set; }
    public string MessagingServiceSid { get; set; }
}