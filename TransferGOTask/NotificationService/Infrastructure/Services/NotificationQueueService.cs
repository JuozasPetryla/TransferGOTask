namespace TransferGOTask.NotificationService.Infrastructure.Services;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Domain.Interfaces;
using Domain.Services;

public class NotificationQueueService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationQueueService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(5);

    public NotificationQueueService(
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationQueueService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger      = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting NotificationQueueService");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                var dispatcher = scope.ServiceProvider.GetRequiredService<NotificationDispatcher>();

                var pending = await repo.GetPendingAsync(stoppingToken);
                if (pending != null)
                {
                    foreach (var n in pending)
                    {
                        try
                        {
                            await dispatcher.DispatchAsync(n, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Dispatch failed for {NotificationId}", n.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error polling notifications");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        _logger.LogInformation("Stopping NotificationQueueService");
    }
}
