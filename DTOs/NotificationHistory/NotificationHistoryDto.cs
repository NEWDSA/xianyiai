namespace XianYu.API.DTOs.NotificationHistory;

public class NotificationHistoryDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public int? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public string Type { get; set; } = null!;
    public string TemplateCode { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Recipient { get; set; }
    public string Status { get; set; } = null!;
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
}

public class NotificationHistoryQueryParams
{
    public int? UserId { get; set; }
    public int? OrderId { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 