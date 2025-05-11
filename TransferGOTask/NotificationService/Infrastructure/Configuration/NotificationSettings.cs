namespace TransferGOTask.NotificationService.Infrastructure.Configuration;

using Domain.Enums;

public class NotificationSettings
{
    public Dictionary<ChannelType, ChannelSettings> Channels { get; set; }
}

public class ChannelSettings
{
    public bool Enabled { get; set; } = true;
    public List<string> Providers { get; set; }
    public Dictionary<string, int> ProviderPriorities { get; set; }
    public Dictionary<string, ProviderSettings> ProviderSettings { get; set; }
}

public class ProviderSettings
{
    public int MaxRetries { get; set; } = 3;
    public int RetryDelaySeconds { get; set; } = 2;
    public int TimeoutSeconds { get; set; } = 30;
}

