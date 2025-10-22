using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class InventoryAlertConfiguration : IEntityTypeConfiguration<InventoryAlert>
{
    public void Configure(EntityTypeBuilder<InventoryAlert> builder)
    {
        builder.ToTable("InventoryAlerts", "Inventory");

        builder.HasKey(ia => ia.Id);
        builder.Property(ia => ia.AlertMessage).IsRequired().HasMaxLength(500);
        builder.Property(ia => ia.AlertDate).IsRequired();
        builder.Property(ia => ia.IsResolved).IsRequired().HasDefaultValue(false);

        builder.HasIndex(ia => ia.ProductId);
        builder.HasIndex(ia => ia.AlertDate);
        builder.HasIndex(ia => ia.IsResolved);

        builder.HasOne(ia => ia.Product).WithMany()
            .HasForeignKey(ia => ia.ProductId).OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(ia => !ia.IsDeleted);
    }
}
