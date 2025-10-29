namespace ShahdCooperative.Application.Events;

public class ProductOutOfStockEvent
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int ThresholdLevel { get; set; }
    public DateTime DetectedAt { get; set; }
}
