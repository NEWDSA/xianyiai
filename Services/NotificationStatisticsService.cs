using Microsoft.EntityFrameworkCore;
using XianYu.API.DTOs.NotificationStatistics;
using XianYu.API.Infrastructure.Data;
using XianYu.API.Models;

namespace XianYu.API.Services;

public interface INotificationStatisticsService
{
    Task<NotificationStatisticsDto> GetStatisticsAsync(NotificationStatisticsQueryParams queryParams);
}

public class NotificationStatisticsService : INotificationStatisticsService
{
    private readonly AppDbContext _context;

    public NotificationStatisticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationStatisticsDto> GetStatisticsAsync(NotificationStatisticsQueryParams queryParams)
    {
        var query = _context.NotificationHistories.AsQueryable();

        // 应用筛选条件
        if (queryParams.StartDate.HasValue)
            query = query.Where(h => h.CreatedAt >= queryParams.StartDate);
        
        if (queryParams.EndDate.HasValue)
            query = query.Where(h => h.CreatedAt <= queryParams.EndDate);
        
        if (!string.IsNullOrEmpty(queryParams.Type))
            query = query.Where(h => h.Type.ToString() == queryParams.Type);
        
        if (queryParams.UserId.HasValue)
            query = query.Where(h => h.UserId == queryParams.UserId);

        // 获取基本统计数据
        var statistics = await query
            .GroupBy(h => 1)
            .Select(g => new
            {
                TotalCount = g.Count(),
                SuccessCount = g.Count(h => h.Status == NotificationStatus.Success),
                FailedCount = g.Count(h => h.Status == NotificationStatus.Failed),
                PendingCount = g.Count(h => h.Status == NotificationStatus.Pending),
                RetryingCount = g.Count(h => h.Status == NotificationStatus.Retrying)
            })
            .FirstOrDefaultAsync() ?? new
            {
                TotalCount = 0,
                SuccessCount = 0,
                FailedCount = 0,
                PendingCount = 0,
                RetryingCount = 0
            };

        // 获取通知类型分布
        var typeDistribution = await query
            .GroupBy(h => h.Type)
            .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Type, x => x.Count);

        // 获取每小时分布
        var hourlyDistribution = await query
            .GroupBy(h => h.CreatedAt.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Hour.ToString("D2"), x => x.Count);

        // 获取每日分布
        var dailyDistribution = await query
            .GroupBy(h => h.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Date.ToString("yyyy-MM-dd"), x => x.Count);

        return new NotificationStatisticsDto
        {
            TotalCount = statistics.TotalCount,
            SuccessCount = statistics.SuccessCount,
            FailedCount = statistics.FailedCount,
            PendingCount = statistics.PendingCount,
            RetryingCount = statistics.RetryingCount,
            SuccessRate = statistics.TotalCount > 0 
                ? (double)statistics.SuccessCount / statistics.TotalCount * 100 
                : 0,
            TypeDistribution = typeDistribution,
            HourlyDistribution = hourlyDistribution,
            DailyDistribution = dailyDistribution
        };
    }
} 