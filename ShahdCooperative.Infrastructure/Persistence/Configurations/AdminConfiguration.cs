using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.ToTable("Admins", "Security");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.ExternalAuthId).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Email).IsRequired().HasMaxLength(255);
        builder.Property(a => a.Role).IsRequired().HasMaxLength(50);
        builder.Property(a => a.Permissions).HasMaxLength(1000);

        builder.HasIndex(a => a.ExternalAuthId).IsUnique();
        builder.HasIndex(a => a.Email);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
