namespace TransferGOTask.NotificationService.Infrastructure.Resilience;

using Microsoft.Extensions.Options;
using Domain.Enums;
using Configuration;
using Polly;
using Polly.Retry;

public class RetryPolicyFactory
{
    private readonly NotificationSettings _settings;

    public RetryPolicyFactory(IOptions<NotificationSettings> options)
    {
        _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }
    
    public AsyncRetryPolicy CreateRetryPolicy(ChannelType channel, string providerKey)
    {
        if (_settings?.Channels == null
            || !_settings.Channels.TryGetValue(channel, out var channelSettings)
            || channelSettings?.ProviderSettings == null
            || !channelSettings.ProviderSettings.TryGetValue(providerKey, out var providerSettings))
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 1,
                    sleepDurationProvider: _ => TimeSpan.FromSeconds(2)
                );
        }
        
        int maxRetries   = providerSettings.MaxRetries;
        int delaySeconds = providerSettings.RetryDelaySeconds;

        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                maxRetries,
                retryAttempt => TimeSpan.FromSeconds(delaySeconds * retryAttempt)
            );
    }
}