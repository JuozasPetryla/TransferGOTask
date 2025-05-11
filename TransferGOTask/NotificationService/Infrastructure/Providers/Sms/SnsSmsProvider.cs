namespace TransferGOTask.NotificationService.Infrastructure.Providers.Sms;

using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using Domain.Entities;
using Configuration;
using Resilience;

public class SnsSmsProvider : BaseSmsProvider
{
    private readonly IAmazonSimpleNotificationService _client;
    public override string Key => "SNS";

    public SnsSmsProvider(
        IAmazonSimpleNotificationService client,
        RetryPolicyFactory retryFactory,
        IOptions<NotificationSettings> settings)
        : base(retryFactory, settings)
    {
        _client = client;
    }

    protected override async Task SendSmsAsync(Notification notification, CancellationToken cancellationToken)
    {
        var request = new PublishRequest
        {
            Message = notification.Message.Body,
            PhoneNumber = notification.Recipient.PhoneNumber
        };
        await _client.PublishAsync(request, cancellationToken);
    }
}