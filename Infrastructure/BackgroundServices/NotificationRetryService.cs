using Microsoft.EntityFrameworkCore;
using XianYu.API.Infrastructure.Data;
using XianYu.API.Models;
using XianYu.API.Services;

namespace XianYu.API.Infrastructure.BackgroundServices;

public class NotificationRetryService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationRetryService> _logger;
    private readonly IConfiguration _configuration;

    public NotificationRetryService(
        IServiceProvider serviceProvider,
        ILogger<NotificationRetryService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessFailedNotificationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理失败通知时发生错误");
            }

            // 等待指定���间后再次检查
            var retryInterval = _configuration.GetValue("NotificationRetry:Interval", 300); // 默认5分钟
            await Task.Delay(TimeSpan.FromSeconds(retryInterval), stoppingToken);
        }
    }

    private async Task ProcessFailedNotificationsAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var historyService = scope.ServiceProvider.GetRequiredService<INotificationHistoryService>();

        var maxRetries = _configuration.GetValue("NotificationRetry:MaxRetries", 3);
        var retryWindow = _configuration.GetValue("NotificationRetry:RetryWindowMinutes", 60);

        // 获取需要重试的通知
        var failedNotifications = await context.NotificationHistories
            .Include(h => h.Order)
            .Include(h => h.User)
            .Where(h => h.Status == NotificationStatus.Failed 
                && h.RetryCount < maxRetries 
                && h.CreatedAt >= DateTime.UtcNow.AddMinutes(-retryWindow))
            .ToListAsync(stoppingToken);

        foreach (var notification in failedNotifications)
        {
            try
            {
                _logger.LogInformation(
                    "正在重试通知 {NotificationId}，重试次数：{RetryCount}", 
                    notification.Id, 
                    notification.RetryCount + 1);

                // 更新状态为重试中
                await historyService.UpdateHistoryStatusAsync(
                    notification.Id, 
                    NotificationStatus.Retrying);

                if (notification.Order != null)
                {
                    // 重新发送通知
                    await notificationService.SendDeliveryNotificationAsync(notification.Order);

                    // 更新状态为成功
                    await historyService.UpdateHistoryStatusAsync(
                        notification.Id, 
                        NotificationStatus.Success);

                    _logger.LogInformation(
                        "通知 {NotificationId} 重试成功", 
                        notification.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "重试通知 {NotificationId} 时发生错误",
                    notification.Id);

                // 更新状态为失败
                await historyService.UpdateHistoryStatusAsync(
                    notification.Id,
                    NotificationStatus.Failed,
                    ex.Message);
            }
        }
    }
} 