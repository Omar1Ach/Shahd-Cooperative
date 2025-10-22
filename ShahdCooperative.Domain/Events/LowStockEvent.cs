namespace ShahdCooperative.Domain.Events;

public record LowStockEvent : DomainEventBase
{
    public Guid ProductId { get; init; }
    public int CurrentStock { get; init; }
    public int ThresholdLevel { get; init; }

    public LowStockEvent(Guid productId, int currentStock, int thresholdLevel)
    {
        ProductId = productId;
        CurrentStock = currentStock;
        ThresholdLevel = thresholdLevel;
    }
}
