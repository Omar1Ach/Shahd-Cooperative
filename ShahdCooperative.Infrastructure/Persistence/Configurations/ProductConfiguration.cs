using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "Inventory");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Category).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Price).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.Currency).IsRequired().HasMaxLength(3).HasDefaultValue("USD");
        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.ThresholdLevel).IsRequired();
        builder.Property(p => p.Type).IsRequired().HasConversion<string>();
        builder.Property(p => p.ImageUrl).HasMaxLength(500);

        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.SKU).IsUnique();
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.Type);

        builder.HasMany(p => p.OrderItems).WithOne(oi => oi.Product)
            .HasForeignKey(oi => oi.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.Feedbacks).WithOne(f => f.Product)
            .HasForeignKey(f => f.ProductId).OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
