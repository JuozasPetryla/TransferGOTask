namespace TransferGOTask.NotificationService.Infrastructure.Providers;

using Polly.Retry;
using Microsoft.Extensions.Options;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Configuration;
using Resilience;

public abstract class BaseEmailProvider : INotificationProvider
{
    public abstract string Key { get; }
    public ChannelType Channel => ChannelType.Email;
    protected readonly AsyncRetryPolicy _retryPolicy;

    protected BaseEmailProvider(RetryPolicyFactory retryFactory, IOptions<NotificationSettings> settings)
    {
        _retryPolicy = retryFactory.CreateRetryPolicy(ChannelType.Email, Key);
    }

    public async Task SendAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            await SendEmailAsync(notification, cancellationToken);
        });
    }

    protected abstract Task SendEmailAsync(Notification notification, CancellationToken cancellationToken);
}