namespace XianYu.API.DTOs.NotificationTemplate;

public class NotificationTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateNotificationTemplateRequest
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateNotificationTemplateRequest
{
    public string? Name { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
} 