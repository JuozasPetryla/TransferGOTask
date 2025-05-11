namespace TransferGOTask.NotificationService.Domain.Interfaces;

using Enums;

public interface IFailoverStrategy
{
    IEnumerable<string> GetProvidersForChannel(ChannelType channel);
}