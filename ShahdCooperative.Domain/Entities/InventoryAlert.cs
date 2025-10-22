using System;

namespace ShahdCooperative.Domain.Entities;

public class InventoryAlert : BaseEntity
{
    public Guid ProductId { get; set; }
    public string AlertMessage { get; set; } = string.Empty;
    public DateTime AlertDate { get; set; } = DateTime.UtcNow;
    public bool IsResolved { get; set; } = false;
    public DateTime? ResolvedAt { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
}
