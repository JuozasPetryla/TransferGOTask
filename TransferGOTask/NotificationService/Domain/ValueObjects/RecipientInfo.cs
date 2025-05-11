namespace TransferGOTask.NotificationService.Domain.ValueObjects;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Owned]
public sealed class RecipientInfo : IEquatable<RecipientInfo>
{
    public RecipientInfo() { }
    
    [Column("Recipient_PhoneNumber")]
    public string PhoneNumber { get; }
    
    [Column("Recipient_Email")]
    public string Email { get; }
    
    
    public RecipientInfo(string phoneNumber = null, string email = null, string deviceToken = null)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)
            && string.IsNullOrWhiteSpace(email)
            && string.IsNullOrWhiteSpace(deviceToken))
        {
            throw new ArgumentException("At least one recipient identifier must be provided.");
        }

        PhoneNumber = phoneNumber;
        Email = email;
    }

    public override bool Equals(object obj) => Equals(obj as RecipientInfo);

    public bool Equals(RecipientInfo other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return PhoneNumber == other.PhoneNumber
               && Email == other.Email;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (PhoneNumber?.GetHashCode() ?? 0);
            hash = hash * 23 + (Email?.GetHashCode() ?? 0);
            return hash;
        }
    }
}