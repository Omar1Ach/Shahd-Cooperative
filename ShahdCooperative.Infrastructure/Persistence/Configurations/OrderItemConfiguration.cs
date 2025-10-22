using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems", "Sales");

        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Quantity).IsRequired();
        builder.Property(oi => oi.UnitPrice).IsRequired().HasPrecision(18, 2);
        builder.Property(oi => oi.Currency).IsRequired().HasMaxLength(3).HasDefaultValue("USD");
        builder.Property(oi => oi.Discount).IsRequired().HasPrecision(18, 2).HasDefaultValue(0);
        builder.Property(oi => oi.Subtotal).IsRequired().HasPrecision(18, 2);

        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.ProductId);

        builder.HasQueryFilter(oi => !oi.IsDeleted);
    }
}
