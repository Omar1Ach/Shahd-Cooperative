namespace ShahdCooperative.Application.DTOs.Products;

public class UpdateProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public int StockQuantity { get; set; }
    public int ThresholdLevel { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}
