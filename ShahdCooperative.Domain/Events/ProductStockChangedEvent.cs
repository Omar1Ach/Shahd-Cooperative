namespace ShahdCooperative.Domain.Events;

public record ProductStockChangedEvent : DomainEventBase
{
    public Guid ProductId { get; init; }
    public int OldStock { get; init; }
    public int NewStock { get; init; }
    public string ChangeReason { get; init; }

    public ProductStockChangedEvent(Guid productId, int oldStock, int newStock, string changeReason)
    {
        ProductId = productId;
        OldStock = oldStock;
        NewStock = newStock;
        ChangeReason = changeReason;
    }
}
