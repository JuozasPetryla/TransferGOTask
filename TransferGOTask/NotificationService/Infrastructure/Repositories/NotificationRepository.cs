namespace TransferGOTask.NotificationService.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using Domain.Entities;
using Domain.Interfaces;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Notification>(b =>
        {
            b.HasKey(n => n.Id);
            b.Property(n => n.Channel).IsRequired();
            b.Property(n => n.Status).IsRequired();
            b.Property(n => n.AttemptCount).IsRequired();
            b.Property(n => n.CreatedAt).IsRequired();
            b.Property(n => n.LastAttemptAt);
            b.Property(n => n.LastError).HasMaxLength(1000).IsRequired(false);
            b.Property(n => n.DeliveredBy)
                .HasMaxLength(200)
                .IsRequired(false);
            
            b.OwnsOne(n => n.Recipient, ri =>
            {
                ri.Property(x => x.PhoneNumber).HasColumnName("Recipient_PhoneNumber");
                ri.Property(x => x.Email).HasColumnName("Recipient_Email");
            });

            b.OwnsOne(n => n.Message, mc =>
            {
                mc.Property(x => x.Subject).HasColumnName("Message_Subject");
                mc.Property(x => x.Body).HasColumnName("Message_Body");
            });
        });
    }
}

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _dbContext;

    public NotificationRepository(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _dbContext.Notifications.AddAsync(notification, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _dbContext.Notifications.Update(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Notification> GetByIdAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications.FindAsync(new object[] { notificationId }, cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .Where(n => n.Status == NotificationStatus.Pending ||
                        n.Status == NotificationStatus.Retrying)
            .ToListAsync(cancellationToken);
    }
}
