namespace ShahdCooperative.Domain.Events;

public record OrderPlacedEvent : DomainEventBase
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime OrderDate { get; init; }

    public OrderPlacedEvent(Guid orderId, Guid customerId, decimal totalAmount, DateTime orderDate)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        OrderDate = orderDate;
    }
}
