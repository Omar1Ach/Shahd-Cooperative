using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", "Core");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.ExternalAuthId).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(255);
        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.Street).HasMaxLength(200);
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.State).HasMaxLength(100);
        builder.Property(c => c.PostalCode).HasMaxLength(20);
        builder.Property(c => c.Country).HasMaxLength(100);
        builder.Property(c => c.LoyaltyPoints).IsRequired().HasDefaultValue(0);
        builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(c => c.DateJoined).IsRequired();

        builder.HasIndex(c => c.ExternalAuthId).IsUnique();
        builder.HasIndex(c => c.Email);

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
