namespace XianYu.API.Models;

public enum NotificationType
{
    Email,
    Sms
}

public class NotificationTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public NotificationType Type { get; set; }
    public string Content { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
} 