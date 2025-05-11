namespace TransferGOTask.NotificationService.Infrastructure.Providers;

using Microsoft.Extensions.Options;
using Polly.Retry;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Configuration;
using Resilience;

public abstract class BaseSmsProvider : INotificationProvider
{
    public abstract string Key { get; }
    public ChannelType Channel => ChannelType.Sms;
    protected readonly AsyncRetryPolicy _retryPolicy;

    protected BaseSmsProvider(
        RetryPolicyFactory retryFactory,
        IOptions<NotificationSettings> settings)
    {
        _retryPolicy = retryFactory.CreateRetryPolicy(ChannelType.Sms, Key);
    }

    public async Task SendAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            await SendSmsAsync(notification, cancellationToken);
        });
    }

    protected abstract Task SendSmsAsync(Notification notification, CancellationToken cancellationToken);
}