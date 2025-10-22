using ShahdCooperative.Domain.Enums;
using System.Collections.Generic;

namespace ShahdCooperative.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public int StockQuantity { get; set; }
    public int ThresholdLevel { get; set; }
    public ProductType Type { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation properties
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
