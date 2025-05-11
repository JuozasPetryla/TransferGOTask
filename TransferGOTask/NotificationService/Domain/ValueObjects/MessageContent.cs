namespace TransferGOTask.NotificationService.Domain.ValueObjects;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Owned]
public sealed class MessageContent : IEquatable<MessageContent>
{
    public MessageContent() { }
    
    [Column("Message_Subject")]
    public string Subject { get; }
    
    [Column("Message_Body")]
    public string Body { get; }
    
    
    public MessageContent(string subject, string body, IDictionary<string, string> metadata = null)
    {
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Message body must be provided.", nameof(body));

        Subject = subject;
        Body = body;
    }

    public override bool Equals(object obj) => Equals(obj as MessageContent);

    public bool Equals(MessageContent other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Subject == other.Subject
               && Body == other.Body;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (Subject?.GetHashCode() ?? 0);
            hash = hash * 23 + Body.GetHashCode();
            return hash;
        }
    }
}