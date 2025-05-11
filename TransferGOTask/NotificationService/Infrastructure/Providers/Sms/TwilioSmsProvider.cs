namespace TransferGOTask.NotificationService.Infrastructure.Providers.Sms;

using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Options;
using Domain.Entities;
using Configuration;
using Resilience;

public class TwilioSmsProvider : BaseSmsProvider
{
    private readonly string _from;
    private readonly string _messagingServiceSid;

    public override string Key => "Twilio";

    public TwilioSmsProvider(
        RetryPolicyFactory retryFactory,
        IOptions<NotificationSettings> settings,
        IOptions<TwilioSettings>    twilioSettings)
        : base(retryFactory, settings)
    {
        var tw = twilioSettings.Value;
        _from               = tw.FromNumber;
        _messagingServiceSid= tw.MessagingServiceSid;
    }

    protected override Task SendSmsAsync(Notification notification, CancellationToken cancellationToken)
    {
        var to = new PhoneNumber(notification.Recipient.PhoneNumber);

        var messageOptions = new CreateMessageOptions(to)
        {
            Body = notification.Message.Body
        };

        if (!string.IsNullOrWhiteSpace(_messagingServiceSid))
            messageOptions.MessagingServiceSid = _messagingServiceSid;
        else
            messageOptions.From = new PhoneNumber(_from);

        return MessageResource.CreateAsync(
            messageOptions
        );
    }
}