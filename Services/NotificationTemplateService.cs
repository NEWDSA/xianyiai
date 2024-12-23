using Microsoft.EntityFrameworkCore;
using XianYu.API.DTOs.NotificationTemplate;
using XianYu.API.Infrastructure.Data;
using XianYu.API.Models;

namespace XianYu.API.Services;

public interface INotificationTemplateService
{
    Task<List<NotificationTemplateDto>> GetTemplatesAsync();
    Task<NotificationTemplateDto?> GetTemplateAsync(int id);
    Task<NotificationTemplateDto?> GetTemplateByCodeAsync(string code);
    Task<NotificationTemplateDto> CreateTemplateAsync(CreateNotificationTemplateRequest request);
    Task<NotificationTemplateDto?> UpdateTemplateAsync(int id, UpdateNotificationTemplateRequest request);
    Task<bool> DeleteTemplateAsync(int id);
}

public class NotificationTemplateService : INotificationTemplateService
{
    private readonly AppDbContext _context;

    public NotificationTemplateService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationTemplateDto>> GetTemplatesAsync()
    {
        return await _context.NotificationTemplates
            .Select(t => new NotificationTemplateDto
            {
                Id = t.Id,
                Name = t.Name,
                Code = t.Code,
                Type = t.Type.ToString(),
                Content = t.Content,
                Description = t.Description,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<NotificationTemplateDto?> GetTemplateAsync(int id)
    {
        var template = await _context.NotificationTemplates.FindAsync(id);
        if (template == null)
            return null;

        return new NotificationTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Code = template.Code,
            Type = template.Type.ToString(),
            Content = template.Content,
            Description = template.Description,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };
    }

    public async Task<NotificationTemplateDto?> GetTemplateByCodeAsync(string code)
    {
        var template = await _context.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Code == code && t.IsActive);
        
        if (template == null)
            return null;

        return new NotificationTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Code = template.Code,
            Type = template.Type.ToString(),
            Content = template.Content,
            Description = template.Description,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };
    }

    public async Task<NotificationTemplateDto> CreateTemplateAsync(CreateNotificationTemplateRequest request)
    {
        var template = new NotificationTemplate
        {
            Name = request.Name,
            Code = request.Code,
            Type = Enum.Parse<NotificationType>(request.Type),
            Content = request.Content,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.NotificationTemplates.Add(template);
        await _context.SaveChangesAsync();

        return new NotificationTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Code = template.Code,
            Type = template.Type.ToString(),
            Content = template.Content,
            Description = template.Description,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };
    }

    public async Task<NotificationTemplateDto?> UpdateTemplateAsync(int id, UpdateNotificationTemplateRequest request)
    {
        var template = await _context.NotificationTemplates.FindAsync(id);
        if (template == null)
            return null;

        if (request.Name != null)
            template.Name = request.Name;
        if (request.Content != null)
            template.Content = request.Content;
        if (request.Description != null)
            template.Description = request.Description;
        if (request.IsActive.HasValue)
            template.IsActive = request.IsActive.Value;

        template.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new NotificationTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Code = template.Code,
            Type = template.Type.ToString(),
            Content = template.Content,
            Description = template.Description,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };
    }

    public async Task<bool> DeleteTemplateAsync(int id)
    {
        var template = await _context.NotificationTemplates.FindAsync(id);
        if (template == null)
            return false;

        _context.NotificationTemplates.Remove(template);
        await _context.SaveChangesAsync();
        return true;
    }
} 