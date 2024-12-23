using Microsoft.EntityFrameworkCore;
using XianYu.API.DTOs.NotificationHistory;
using XianYu.API.Infrastructure.Data;
using XianYu.API.Models;

namespace XianYu.API.Services;

public interface INotificationHistoryService
{
    Task<(List<NotificationHistoryDto> Items, int Total)> GetHistoryAsync(NotificationHistoryQueryParams queryParams);
    Task<NotificationHistoryDto?> GetHistoryByIdAsync(int id);
    Task<NotificationHistoryDto> CreateHistoryAsync(NotificationHistory history);
    Task<NotificationHistoryDto?> UpdateHistoryStatusAsync(int id, NotificationStatus status, string? errorMessage = null);
}

public class NotificationHistoryService : INotificationHistoryService
{
    private readonly AppDbContext _context;

    public NotificationHistoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<NotificationHistoryDto> Items, int Total)> GetHistoryAsync(NotificationHistoryQueryParams queryParams)
    {
        var query = _context.NotificationHistories
            .Include(h => h.User)
            .Include(h => h.Order)
            .AsQueryable();

        // 应用筛选条���
        if (queryParams.UserId.HasValue)
            query = query.Where(h => h.UserId == queryParams.UserId);
        
        if (queryParams.OrderId.HasValue)
            query = query.Where(h => h.OrderId == queryParams.OrderId);
        
        if (!string.IsNullOrEmpty(queryParams.Type))
            query = query.Where(h => h.Type.ToString() == queryParams.Type);
        
        if (!string.IsNullOrEmpty(queryParams.Status))
            query = query.Where(h => h.Status.ToString() == queryParams.Status);
        
        if (queryParams.StartDate.HasValue)
            query = query.Where(h => h.CreatedAt >= queryParams.StartDate);
        
        if (queryParams.EndDate.HasValue)
            query = query.Where(h => h.CreatedAt <= queryParams.EndDate);

        // 获取总记录数
        var total = await query.CountAsync();

        // 应用分页
        var items = await query
            .OrderByDescending(h => h.CreatedAt)
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .Select(h => new NotificationHistoryDto
            {
                Id = h.Id,
                UserId = h.UserId,
                Username = h.User.Username,
                OrderId = h.OrderId,
                OrderNumber = h.Order != null ? h.Order.OrderNumber : null,
                Type = h.Type.ToString(),
                TemplateCode = h.TemplateCode,
                Content = h.Content,
                Recipient = h.Recipient,
                Status = h.Status.ToString(),
                ErrorMessage = h.ErrorMessage,
                RetryCount = h.RetryCount,
                CreatedAt = h.CreatedAt,
                SentAt = h.SentAt
            })
            .ToListAsync();

        return (items, total);
    }

    public async Task<NotificationHistoryDto?> GetHistoryByIdAsync(int id)
    {
        var history = await _context.NotificationHistories
            .Include(h => h.User)
            .Include(h => h.Order)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (history == null)
            return null;

        return new NotificationHistoryDto
        {
            Id = history.Id,
            UserId = history.UserId,
            Username = history.User.Username,
            OrderId = history.OrderId,
            OrderNumber = history.Order?.OrderNumber,
            Type = history.Type.ToString(),
            TemplateCode = history.TemplateCode,
            Content = history.Content,
            Recipient = history.Recipient,
            Status = history.Status.ToString(),
            ErrorMessage = history.ErrorMessage,
            RetryCount = history.RetryCount,
            CreatedAt = history.CreatedAt,
            SentAt = history.SentAt
        };
    }

    public async Task<NotificationHistoryDto> CreateHistoryAsync(NotificationHistory history)
    {
        _context.NotificationHistories.Add(history);
        await _context.SaveChangesAsync();

        return await GetHistoryByIdAsync(history.Id) ?? 
            throw new InvalidOperationException("Failed to create notification history");
    }

    public async Task<NotificationHistoryDto?> UpdateHistoryStatusAsync(int id, NotificationStatus status, string? errorMessage = null)
    {
        var history = await _context.NotificationHistories.FindAsync(id);
        if (history == null)
            return null;

        history.Status = status;
        if (errorMessage != null)
            history.ErrorMessage = errorMessage;

        if (status == NotificationStatus.Success)
            history.SentAt = DateTime.UtcNow;
        else if (status == NotificationStatus.Retrying)
            history.RetryCount++;

        await _context.SaveChangesAsync();

        return await GetHistoryByIdAsync(id);
    }
} 