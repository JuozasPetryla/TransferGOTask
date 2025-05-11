using Polly.Retry;

namespace TransferGOTask.NotificationService.Application.Services;

using TransferGOTask.NotificationService.Domain.Interfaces;
using Infrastructure.Resilience;
using DTOs;
using Exceptions;
using Interfaces;
using Domain.Enums;
public class NotificationService : INotificationService
{
    private readonly INotificationRepository           _repository;
    private readonly IFailoverStrategy                 _failover;
    private readonly RetryPolicyFactory                _retryFactory;
    private readonly IEnumerable<INotificationProvider> _providers;
    private readonly INotificationRequestDtoMapper     _mapper;

    public NotificationService(
        INotificationRepository           repository,
        IFailoverStrategy                 failover,
        RetryPolicyFactory                retryFactory,
        IEnumerable<INotificationProvider> providers,
        INotificationRequestDtoMapper     mapper)
    {
        _repository   = repository;
        _failover     = failover;
        _retryFactory = retryFactory;
        _providers    = providers;
        _mapper       = mapper;
    }

    public async Task<NotificationResponseDto> SendNotificationAsync(
        NotificationRequestDto request,
        CancellationToken      cancellationToken = default)
    {
        var notification = _mapper.Map(request);
        await _repository.AddAsync(notification, cancellationToken);

        Exception lastEx = null;
        var providerKeys = _failover.GetProvidersForChannel(request.Channel);

        foreach (var key in providerKeys)
        {
            var provider = _providers.Single(p => p.Key == key);
            var policy   = _retryFactory.CreateRetryPolicy(request.Channel, key);
            
            notification.StartProcessing();
            await _repository.UpdateAsync(notification, cancellationToken);

            try
            {
                await policy.ExecuteAsync(() =>
                    provider.SendAsync(notification, cancellationToken)
                );

                notification.MarkSent(key);
                await _repository.UpdateAsync(notification, cancellationToken);

                return new NotificationResponseDto
                {
                    NotificationId = notification.Id,
                    Status         = notification.Status,
                    DeliveredBy    = notification.DeliveredBy
                };
            }
            catch (Exception ex)
            {
                lastEx = ex;
                notification.MarkFailed(ex.Message);
                await _repository.UpdateAsync(notification, cancellationToken);
            }
        }
        
        notification.FailPermanently(lastEx?.Message ?? "All providers failed");
        await _repository.UpdateAsync(notification, cancellationToken);
        
        throw new NotificationFailedException(
            $"Failed to deliver notification with id {notification.Id} via: {string.Join(", ", providerKeys)}",
            lastEx
        );
    }

    public async Task<NotificationStatus> GetStatusAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        var notification = await _repository.GetByIdAsync(notificationId, cancellationToken);
        if (notification == null)
            throw new NotificationNotFoundException(notificationId);

        return notification.Status;
    }
}