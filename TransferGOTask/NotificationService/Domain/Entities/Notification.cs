namespace TransferGOTask.NotificationService.Domain.Entities;

using Enums;
using ValueObjects;

public class Notification
{
    public Guid Id { get; private set; }
    public ChannelType Channel { get; private set; }
    public RecipientInfo Recipient { get; private set; }
    public MessageContent Message { get; private set; }
    public NotificationStatus Status { get; private set; }
    public int AttemptCount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastAttemptAt { get; private set; }
    public string? LastError { get; private set; }

    private const int MaxRetryAttempts = 3;
    public string? DeliveredBy { get; private set; } 
    
    private Notification() { }
    
    public Notification(ChannelType channel, RecipientInfo recipient, MessageContent message)
    {
        Id = Guid.NewGuid();
        Channel = channel;
        Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Status = NotificationStatus.Pending;
        AttemptCount = 0;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void StartProcessing()
    {
        if (Status != NotificationStatus.Pending
            && Status != NotificationStatus.Retrying)
        {
            throw new InvalidOperationException(
                "Cannot start processing unless Pending or Retrying.");
        }
        
        LastError      = null;
        Status         = NotificationStatus.InProgress;
        AttemptCount  += 1;
        LastAttemptAt  = DateTime.UtcNow;
    }

    public void MarkSent(string providerKey)
    {
        if (Status != NotificationStatus.InProgress)
            throw new InvalidOperationException(
                "Can only mark as Sent when InProgress.");

        Status      = NotificationStatus.Sent;
        DeliveredBy = providerKey;
    }

    public void MarkFailed(string error)
    {
        if (Status != NotificationStatus.InProgress)
            throw new InvalidOperationException(
                "Can only mark as Failed when InProgress.");

        LastError = error;
        
        Status = (AttemptCount < MaxRetryAttempts)
            ? NotificationStatus.Retrying
            : NotificationStatus.Failed;
    }
    
    public void FailPermanently(string error)
    {
        if (Status == NotificationStatus.Pending)
            throw new InvalidOperationException("Cannot permanently fail before any attempt.");

        LastError     = error;
        Status        = NotificationStatus.Failed;
    }
}

