namespace XianYu.API.DTOs.NotificationStatistics;

public class NotificationStatisticsDto
{
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public int PendingCount { get; set; }
    public int RetryingCount { get; set; }
    public double SuccessRate { get; set; }
    public Dictionary<string, int> TypeDistribution { get; set; } = new();
    public Dictionary<string, int> HourlyDistribution { get; set; } = new();
    public Dictionary<string, int> DailyDistribution { get; set; } = new();
}

public class NotificationStatisticsQueryParams
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Type { get; set; }
    public int? UserId { get; set; }
} 