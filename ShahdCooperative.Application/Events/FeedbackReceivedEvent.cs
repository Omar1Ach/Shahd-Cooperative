namespace ShahdCooperative.Application.Events;

public class FeedbackReceivedEvent
{
    public Guid FeedbackId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }
    public Guid? OrderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime SubmittedAt { get; set; }
}
