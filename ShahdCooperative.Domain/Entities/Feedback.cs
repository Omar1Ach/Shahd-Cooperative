using System;

namespace ShahdCooperative.Domain.Entities;

public class Feedback : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? OrderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Response { get; set; }
    public Guid? RespondedBy { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string Status { get; set; } = "Pending";

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public Product? Product { get; set; }
    public Order? Order { get; set; }
    public Admin? Responder { get; set; }
}
