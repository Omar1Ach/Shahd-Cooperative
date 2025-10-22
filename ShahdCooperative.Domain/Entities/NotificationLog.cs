using ShahdCooperative.Domain.Enums;
using System;

namespace ShahdCooperative.Domain.Entities;

public class NotificationLog : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string? RecipientPhone { get; set; }
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public NotificationStatus Status { get; set; }
    public int RetryCount { get; set; } = 0;
    public string? Metadata { get; set; }
}
