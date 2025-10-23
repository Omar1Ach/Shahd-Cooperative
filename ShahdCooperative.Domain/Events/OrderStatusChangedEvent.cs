using ShahdCooperative.Domain.Enums;

namespace ShahdCooperative.Domain.Events;

public record OrderStatusChangedEvent : DomainEventBase
{
    public Guid OrderId { get; init; }
    public OrderStatus OldStatus { get; init; }
    public OrderStatus NewStatus { get; init; }
    public DateTime ChangedAt { get; init; }

    public OrderStatusChangedEvent(Guid orderId, OrderStatus oldStatus, OrderStatus newStatus, DateTime changedAt)
    {
        OrderId = orderId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ChangedAt = changedAt;
    }
}
