# ShahdCooperative - Complete File Generation Script
# This script generates all necessary files for the Clean Architecture solution

Write-Host "Generating ShahdCooperative files..." -ForegroundColor Cyan

# Helper function to create files
function Create-File {
    param(
        [string]$Path,
        [string]$Content
    )
    $Content | Out-File -FilePath $Path -Encoding UTF8
    Write-Host "Created: $Path" -ForegroundColor Gray
}

Write-Host "`n=== Domain Layer ===" -ForegroundColor Yellow

# Base Entity
$baseEntity = @"
using System;

namespace ShahdCooperative.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}
"@
Create-File -Path "ShahdCooperative.Domain\Entities\BaseEntity.cs" -Content $baseEntity

# Product Entity
$productEntity = @"
using ShahdCooperative.Domain.Enums;
using System.Collections.Generic;

namespace ShahdCooperative.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public ProductType Type { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation properties
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
"@
Create-File -Path "ShahdCooperative.Domain\Entities\Product.cs" -Content $productEntity

# Order Entity
$orderEntity = @"
using ShahdCooperative.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ShahdCooperative.Domain.Entities;

public class Order : BaseEntity
{
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
"@
Create-File -Path "ShahdCooperative.Domain\Entities\Order.cs" -Content $orderEntity

# OrderItem Entity
$orderItemEntity = @"
using System;

namespace ShahdCooperative.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
"@
Create-File -Path "ShahdCooperative.Domain\Entities\OrderItem.cs" -Content $orderItemEntity

# Customer Entity
$customerEntity = @"
using System;
using System.Collections.Generic;

namespace ShahdCooperative.Domain.Entities;

public class Customer : BaseEntity
{
    public string ExternalAuthId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
"@
Create-File -Path "ShahdCooperative.Domain\Entities\Customer.cs" -Content $customerEntity

# Admin Entity
$adminEntity = @"
namespace ShahdCooperative.Domain.Entities;

public class Admin : BaseEntity
{
    public string ExternalAuthId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Permissions { get; set; }
}
"@
Create-File -Path "ShahdCooperative.Domain\Entities\Admin.cs" -Content $adminEntity

# NotificationLog Entity
$notificationLogEntity = @"
using ShahdCooperative.Domain.Enums;
using System;

namespace ShahdCooperative.Domain.Entities;

public class NotificationLog : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public NotificationStatus Status { get; set; }
}
"@
Create-File -Path "ShahdCooperative.Domain\Entities\NotificationLog.cs" -Content $notificationLogEntity

# Feedback Entity
$feedbackEntity = @"
using System;

namespace ShahdCooperative.Domain.Entities;

public class Feedback : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid? ProductId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public Product? Product { get; set; }
}
"@
Create-File -Path "ShahdCooperative.Domain\Entities\Feedback.cs" -Content $feedbackEntity

# Enums
$productType = @"
namespace ShahdCooperative.Domain.Enums;

public enum ProductType
{
    BeeProduct,
    Equipment
}
"@
Create-File -Path "ShahdCooperative.Domain\Enums\ProductType.cs" -Content $productType

$orderStatus = @"
namespace ShahdCooperative.Domain.Enums;

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}
"@
Create-File -Path "ShahdCooperative.Domain\Enums\OrderStatus.cs" -Content $orderStatus

$notificationType = @"
namespace ShahdCooperative.Domain.Enums;

public enum NotificationType
{
    Email,
    SMS,
    Push
}
"@
Create-File -Path "ShahdCooperative.Domain\Enums\NotificationType.cs" -Content $notificationType

$notificationStatus = @"
namespace ShahdCooperative.Domain.Enums;

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed
}
"@
Create-File -Path "ShahdCooperative.Domain\Enums\NotificationStatus.cs" -Content $notificationStatus

# Generic Repository Interface
$iRepository = @"
using ShahdCooperative.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
"@
Create-File -Path "ShahdCooperative.Domain\Interfaces\Repositories\IRepository.cs" -Content $iRepository

# Specific Repository Interfaces
$iProductRepository = @"
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByTypeAsync(ProductType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold, CancellationToken cancellationToken = default);
}
"@
Create-File -Path "ShahdCooperative.Domain\Interfaces\Repositories\IProductRepository.cs" -Content $iProductRepository

$iOrderRepository = @"
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
}
"@
Create-File -Path "ShahdCooperative.Domain\Interfaces\Repositories\IOrderRepository.cs" -Content $iOrderRepository

$iCustomerRepository = @"
using ShahdCooperative.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByAuthIdAsync(string authId, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
"@
Create-File -Path "ShahdCooperative.Domain\Interfaces\Repositories\ICustomerRepository.cs" -Content $iCustomerRepository

$iFeedbackRepository = @"
using ShahdCooperative.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Repositories;

public interface IFeedbackRepository : IRepository<Feedback>
{
    Task<IEnumerable<Feedback>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Feedback>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
"@
Create-File -Path "ShahdCooperative.Domain\Interfaces\Repositories\IFeedbackRepository.cs" -Content $iFeedbackRepository

# Unit of Work Interface
$iUnitOfWork = @"
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    ICustomerRepository Customers { get; }
    IFeedbackRepository Feedbacks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
"@
Create-File -Path "ShahdCooperative.Domain\Interfaces\Repositories\IUnitOfWork.cs" -Content $iUnitOfWork

# Service Interfaces
$iAuthService = @"
using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Services;

public interface IAuthService
{
    Task<bool> ValidateTokenAsync(string token);
    Task<string?> GetUserIdFromTokenAsync(string token);
}
"@
Create-File -Path "ShahdCooperative.Domain\Interfaces\Services\IAuthService.cs" -Content $iAuthService

$iNotificationService = @"
using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Services;

public interface INotificationService
{
    Task SendEmailAsync(string recipient, string subject, string body);
    Task SendSmsAsync(string phoneNumber, string message);
    Task SendPushNotificationAsync(string userId, string message);
}
"@
Create-File -Path "ShahdCooperative.Domain\Interfaces\Services\INotificationService.cs" -Content $iNotificationService

# Domain Exceptions
$domainException = @"
using System;

namespace ShahdCooperative.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
"@
Create-File -Path "ShahdCooperative.Domain\Exceptions\DomainException.cs" -Content $domainException

Write-Host "`n=== Domain Layer Complete ===" -ForegroundColor Green
Write-Host "All files generated successfully!" -ForegroundColor Cyan
