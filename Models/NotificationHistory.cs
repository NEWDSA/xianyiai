namespace XianYu.API.Models;

public enum NotificationStatus
{
    Success,
    Failed,
    Pending,
    Retrying
}

public class NotificationHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
    public NotificationType Type { get; set; }
    public string TemplateCode { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Recipient { get; set; }
    public NotificationStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
} 