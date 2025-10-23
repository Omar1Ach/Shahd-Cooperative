using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Events;

namespace ShahdCooperative.Domain.Entities;

public class Product : BaseEntity
{
    // Private setters for encapsulation
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = "USD";
    public int StockQuantity { get; private set; }
    public int ThresholdLevel { get; private set; }
    public ProductType Type { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
    public ICollection<Feedback> Feedbacks { get; private set; } = new List<Feedback>();

    // Parameterless constructor for EF Core
    private Product() { }

    // Factory method for creating products
    public static Product Create(
        string name,
        string sku,
        string category,
        ProductType type,
        decimal price,
        string currency,
        int stockQuantity,
        int thresholdLevel,
        string? description = null,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be empty", nameof(sku));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));

        return new Product
        {
            Name = name,
            SKU = sku,
            Category = category,
            Type = type,
            Price = price,
            Currency = currency,
            StockQuantity = stockQuantity,
            ThresholdLevel = thresholdLevel,
            Description = description,
            ImageUrl = imageUrl
        };
    }

    // Business methods
    public void UpdateStock(int quantity, string reason)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot update stock for inactive product");

        var oldStock = StockQuantity;
        StockQuantity = quantity;

        if (StockQuantity < 0)
            StockQuantity = 0;

        AddDomainEvent(new ProductStockChangedEvent(Id, oldStock, StockQuantity, reason));

        if (ShouldTriggerLowStockAlert())
        {
            AddDomainEvent(new LowStockEvent(Id, StockQuantity, ThresholdLevel));
        }
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (!CanFulfillOrder(quantity))
            throw new InvalidOperationException($"Insufficient stock. Available: {StockQuantity}, Requested: {quantity}");

        UpdateStock(StockQuantity - quantity, $"Stock reduced by {quantity}");
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        UpdateStock(StockQuantity + quantity, $"Stock increased by {quantity}");
    }

    public bool IsInStock() => StockQuantity > 0 && IsActive;

    public bool CanFulfillOrder(int requestedQuantity) => StockQuantity >= requestedQuantity && IsActive;

    public bool ShouldTriggerLowStockAlert() => StockQuantity <= ThresholdLevel && IsActive;

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void UpdateDetails(
        string name,
        string category,
        decimal price,
        string? description = null,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        Name = name;
        Category = category;
        Price = price;
        Description = description;
        ImageUrl = imageUrl;
    }

    public void UpdateThresholdLevel(int thresholdLevel)
    {
        if (thresholdLevel < 0)
            throw new ArgumentException("Threshold level cannot be negative", nameof(thresholdLevel));

        ThresholdLevel = thresholdLevel;

        if (ShouldTriggerLowStockAlert())
        {
            AddDomainEvent(new LowStockEvent(Id, StockQuantity, ThresholdLevel));
        }
    }
}
