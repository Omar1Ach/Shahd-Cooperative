using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", "Sales");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
        builder.Property(o => o.OrderDate).IsRequired();
        builder.Property(o => o.Status).IsRequired().HasConversion<string>();
        builder.Property(o => o.TotalAmount).IsRequired().HasPrecision(18, 2);
        builder.Property(o => o.Currency).IsRequired().HasMaxLength(3).HasDefaultValue("USD");
        builder.Property(o => o.ShippingStreet).HasMaxLength(200);
        builder.Property(o => o.ShippingCity).HasMaxLength(100);
        builder.Property(o => o.ShippingState).HasMaxLength(100);
        builder.Property(o => o.ShippingPostalCode).HasMaxLength(20);
        builder.Property(o => o.ShippingCountry).HasMaxLength(100);
        builder.Property(o => o.TrackingNumber).HasMaxLength(100);

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.OrderDate);
        builder.HasIndex(o => o.Status);

        builder.HasOne(o => o.Customer).WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(o => o.OrderItems).WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId).OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}
