namespace ShahdCooperative.Application.DTOs.Feedback;

public class FeedbackDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? OrderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Response { get; set; }
    public Guid? RespondedBy { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
