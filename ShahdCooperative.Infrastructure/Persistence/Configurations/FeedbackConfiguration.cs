using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.ToTable("Feedback", "Core");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Content).IsRequired().HasMaxLength(2000);
        builder.Property(f => f.Rating).IsRequired();
        builder.Property(f => f.Response).HasMaxLength(2000);
        builder.Property(f => f.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Pending");

        builder.HasIndex(f => f.CustomerId);
        builder.HasIndex(f => f.ProductId);
        builder.HasIndex(f => f.OrderId);

        builder.HasOne(f => f.Customer).WithMany(c => c.Feedbacks)
            .HasForeignKey(f => f.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(f => f.Order).WithMany()
            .HasForeignKey(f => f.OrderId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(f => f.Responder).WithMany()
            .HasForeignKey(f => f.RespondedBy).OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(f => !f.IsDeleted);
    }
}
