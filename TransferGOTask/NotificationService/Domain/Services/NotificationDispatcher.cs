namespace TransferGOTask.NotificationService.Domain.Services;

using Entities;
using Enums;
using Interfaces;

public class NotificationDispatcher
{
    private readonly IEnumerable<INotificationProvider> _providers;
    private readonly IFailoverStrategy _failoverStrategy;
    private readonly INotificationRepository _repository;

    public NotificationDispatcher(
        IEnumerable<INotificationProvider> providers,
        IFailoverStrategy failoverStrategy,
        INotificationRepository repository)
    {
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        _failoverStrategy = failoverStrategy ?? throw new ArgumentNullException(nameof(failoverStrategy));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    public async Task DispatchAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        if (notification == null) throw new ArgumentNullException(nameof(notification));
        
        var providerKeys = _failoverStrategy.GetProvidersForChannel(notification.Channel);

        foreach (var key in providerKeys)
        {
            var provider = _providers.SingleOrDefault(p => p.Key.Equals(key, StringComparison.OrdinalIgnoreCase)
                                                           && p.Channel == notification.Channel);
            if (provider == null) continue;
            
            notification.StartProcessing();
            await _repository.UpdateAsync(notification, cancellationToken);

            try
            {
                await provider.SendAsync(notification, cancellationToken);
                
                notification.MarkSent(provider.Key);
                await _repository.UpdateAsync(notification, cancellationToken);
                return;
            }
            catch (Exception ex)
            {
                notification.MarkFailed(ex.Message);
                await _repository.UpdateAsync(notification, cancellationToken);
                
                if (notification.Status == NotificationStatus.Retrying)
                    return;
            }
        }
        
        if (notification.Status == NotificationStatus.InProgress)
        {
            notification.MarkFailed("No providers available for channel.");
            await _repository.UpdateAsync(notification, cancellationToken);
        }
    }
}

