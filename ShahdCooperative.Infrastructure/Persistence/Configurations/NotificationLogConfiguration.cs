using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.ToTable("NotificationLogs", "Notification");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.UserId).IsRequired().HasMaxLength(100);
        builder.Property(n => n.RecipientEmail).IsRequired().HasMaxLength(255);
        builder.Property(n => n.RecipientPhone).HasMaxLength(20);
        builder.Property(n => n.Subject).HasMaxLength(200);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
        builder.Property(n => n.Type).IsRequired().HasConversion<string>();
        builder.Property(n => n.SentAt).IsRequired();
        builder.Property(n => n.Status).IsRequired().HasConversion<string>();
        builder.Property(n => n.RetryCount).IsRequired().HasDefaultValue(0);
        builder.Property(n => n.Metadata).HasMaxLength(2000);

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.SentAt);
        builder.HasIndex(n => n.Status);

        builder.HasQueryFilter(n => !n.IsDeleted);
    }
}
