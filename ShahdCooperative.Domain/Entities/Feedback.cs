using ShahdCooperative.Domain.Events;

namespace ShahdCooperative.Domain.Entities;

public class Feedback : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid? OrderId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public int Rating { get; private set; }
    public string? Response { get; private set; }
    public Guid? RespondedBy { get; private set; }
    public DateTime? RespondedAt { get; private set; }
    public string Status { get; private set; } = "Pending";

    // Navigation properties
    public Customer Customer { get; private set; } = null!;
    public Product? Product { get; private set; }
    public Order? Order { get; private set; }
    public Admin? Responder { get; private set; }

    // Parameterless constructor for EF Core
    private Feedback() { }

    // Factory method
    public static Feedback Create(
        Guid customerId,
        string content,
        int rating,
        Guid? productId = null,
        Guid? orderId = null)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));

        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

        var feedback = new Feedback
        {
            CustomerId = customerId,
            ProductId = productId,
            OrderId = orderId,
            Content = content,
            Rating = rating,
            Status = "Pending"
        };

        feedback.AddDomainEvent(new FeedbackSubmittedEvent(
            feedback.Id,
            productId,
            customerId,
            rating));

        return feedback;
    }

    // Business methods
    public void Respond(Guid adminId, string response)
    {
        if (adminId == Guid.Empty)
            throw new ArgumentException("Admin ID cannot be empty", nameof(adminId));

        if (string.IsNullOrWhiteSpace(response))
            throw new ArgumentException("Response cannot be empty", nameof(response));

        if (Status == "Responded")
            throw new InvalidOperationException("Feedback has already been responded to");

        Response = response;
        RespondedBy = adminId;
        RespondedAt = DateTime.UtcNow;
        Status = "Responded";
    }

    public void MarkAsResolved()
    {
        Status = "Resolved";
    }
}
