namespace TransferGOTask.NotificationService.Infrastructure.Resilience;

using Microsoft.Extensions.Options;
using Domain.Enums;
using Domain.Interfaces;
using Configuration;

public class FailoverPolicy : IFailoverStrategy
{
    private readonly NotificationSettings _settings;

    public FailoverPolicy(IOptions<NotificationSettings> options)
    {
        _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }
    
    public IEnumerable<string> GetProvidersForChannel(ChannelType channel)
    {
        if (!_settings.Channels.TryGetValue(channel, out var channelSettings)
            || !channelSettings.Enabled)
        {
            return Enumerable.Empty<string>();
        }
        
        return channelSettings.Providers
            .Where(p => channelSettings.ProviderPriorities.ContainsKey(p))
            .OrderBy(p => channelSettings.ProviderPriorities[p]);
    }
}