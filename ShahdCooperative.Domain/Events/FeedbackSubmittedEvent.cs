namespace ShahdCooperative.Domain.Events;

public record FeedbackSubmittedEvent : DomainEventBase
{
    public Guid FeedbackId { get; init; }
    public Guid? ProductId { get; init; }
    public Guid CustomerId { get; init; }
    public int Rating { get; init; }

    public FeedbackSubmittedEvent(Guid feedbackId, Guid? productId, Guid customerId, int rating)
    {
        FeedbackId = feedbackId;
        ProductId = productId;
        CustomerId = customerId;
        Rating = rating;
    }
}
