# ShahdCooperative - Infrastructure Layer Setup Script
Write-Host "Setting up Infrastructure Layer..." -ForegroundColor Cyan

function Create-File {
    param([string]$Path, [string]$Content)
    $Content | Out-File -FilePath $Path -Encoding UTF8
    Write-Host "Created: $Path" -ForegroundColor Gray
}

# Remove default class
Remove-Item "ShahdCooperative.Infrastructure\Class1.cs" -ErrorAction SilentlyContinue
Remove-Item "ShahdCooperative.Application\Class1.cs" -ErrorAction SilentlyContinue

Write-Host "`n=== Creating ApplicationDbContext ===" -ForegroundColor Yellow

$dbContext = @"
using Microsoft.EntityFrameworkCore;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\ApplicationDbContext.cs" -Content $dbContext

Write-Host "`n=== Creating Entity Configurations ===" -ForegroundColor Yellow

$productConfig = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.Category).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Price).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.Type).IsRequired().HasConversion<string>();
        builder.Property(p => p.ImageUrl).HasMaxLength(500);

        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.Type);

        builder.HasMany(p => p.OrderItems).WithOne(oi => oi.Product)
            .HasForeignKey(oi => oi.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.Feedbacks).WithOne(f => f.Product)
            .HasForeignKey(f => f.ProductId).OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\Configurations\ProductConfiguration.cs" -Content $productConfig

$orderConfig = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderDate).IsRequired();
        builder.Property(o => o.Status).IsRequired().HasConversion<string>();
        builder.Property(o => o.TotalAmount).IsRequired().HasPrecision(18, 2);
        builder.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(500);
        builder.Property(o => o.TrackingNumber).HasMaxLength(100);

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
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\Configurations\OrderConfiguration.cs" -Content $orderConfig

$orderItemConfig = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Quantity).IsRequired();
        builder.Property(oi => oi.UnitPrice).IsRequired().HasPrecision(18, 2);
        builder.Property(oi => oi.Subtotal).IsRequired().HasPrecision(18, 2);

        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.ProductId);

        builder.HasQueryFilter(oi => !oi.IsDeleted);
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\Configurations\OrderItemConfiguration.cs" -Content $orderItemConfig

$customerConfig = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.ExternalAuthId).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(255);
        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.DateJoined).IsRequired();

        builder.HasIndex(c => c.ExternalAuthId).IsUnique();
        builder.HasIndex(c => c.Email);

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\Configurations\CustomerConfiguration.cs" -Content $customerConfig

$adminConfig = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
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
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\Configurations\AdminConfiguration.cs" -Content $adminConfig

$notificationConfig = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.UserId).IsRequired().HasMaxLength(100);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
        builder.Property(n => n.Type).IsRequired().HasConversion<string>();
        builder.Property(n => n.SentAt).IsRequired();
        builder.Property(n => n.Status).IsRequired().HasConversion<string>();

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.SentAt);
        builder.HasIndex(n => n.Status);

        builder.HasQueryFilter(n => !n.IsDeleted);
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\Configurations\NotificationLogConfiguration.cs" -Content $notificationConfig

$feedbackConfig = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Infrastructure.Persistence.Configurations;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Content).IsRequired().HasMaxLength(2000);
        builder.Property(f => f.Rating).IsRequired();

        builder.HasIndex(f => f.CustomerId);
        builder.HasIndex(f => f.ProductId);

        builder.HasOne(f => f.Customer).WithMany(c => c.Feedbacks)
            .HasForeignKey(f => f.CustomerId).OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(f => !f.IsDeleted);
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\Configurations\FeedbackConfiguration.cs" -Content $feedbackConfig

Write-Host "`n=== Creating Repositories ===" -ForegroundColor Yellow

$repository = @"
using Microsoft.EntityFrameworkCore;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;
using System.Linq.Expressions;

namespace ShahdCooperative.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\Repositories\Repository.cs" -Content $repository

$productRepo = @"
using Microsoft.EntityFrameworkCore;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetByTypeAsync(
        ProductType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => p.Type == type).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(
        string category, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => p.Category == category).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(
        int threshold, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => p.StockQuantity <= threshold)
            .OrderBy(p => p.StockQuantity).ToListAsync(cancellationToken);
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\Repositories\ProductRepository.cs" -Content $productRepo

$orderRepo = @"
using Microsoft.EntityFrameworkCore;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(o => o.CustomerId == customerId)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(
        OrderStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(o => o.Status == status)
            .Include(o => o.OrderItems).OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdWithItemsAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.Customer).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\Repositories\OrderRepository.cs" -Content $orderRepo

$customerRepo = @"
using Microsoft.EntityFrameworkCore;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Customer?> GetByAuthIdAsync(
        string authId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.ExternalAuthId == authId, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(
        string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\Repositories\CustomerRepository.cs" -Content $customerRepo

$feedbackRepo = @"
using Microsoft.EntityFrameworkCore;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence.Repositories;

public class FeedbackRepository : Repository<Feedback>, IFeedbackRepository
{
    public FeedbackRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Feedback>> GetByProductIdAsync(
        Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(f => f.ProductId == productId)
            .Include(f => f.Customer).OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Feedback>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(f => f.CustomerId == customerId)
            .Include(f => f.Product).OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\Repositories\FeedbackRepository.cs" -Content $feedbackRepo

$unitOfWork = @"
using Microsoft.EntityFrameworkCore.Storage;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context,
        IProductRepository products, IOrderRepository orders,
        ICustomerRepository customers, IFeedbackRepository feedbacks)
    {
        _context = context;
        Products = products;
        Orders = orders;
        Customers = customers;
        Feedbacks = feedbacks;
    }

    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    public ICustomerRepository Customers { get; }
    public IFeedbackRepository Feedbacks { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\Persistence\UnitOfWork.cs" -Content $unitOfWork

Write-Host "`n=== Creating DependencyInjection ===" -ForegroundColor Yellow

$infraDI = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShahdCooperative.Domain.Interfaces.Repositories;
using ShahdCooperative.Infrastructure.Persistence;
using ShahdCooperative.Infrastructure.Persistence.Repositories;

namespace ShahdCooperative.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
"@
Create-File -Path "ShahdCooperative.Infrastructure\DependencyInjection.cs" -Content $infraDI

Write-Host "`n=== Infrastructure Layer Complete! ===" -ForegroundColor Green
Write-Host "Files created: 16" -ForegroundColor Cyan
