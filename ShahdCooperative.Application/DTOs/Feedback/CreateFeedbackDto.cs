namespace ShahdCooperative.Application.DTOs.Feedback;

public class CreateFeedbackDto
{
    public Guid CustomerId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? OrderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
}
