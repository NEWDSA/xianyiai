using XianYu.API.Models;

namespace XianYu.API.Services;

public interface INotificationService
{
    Task SendDeliveryNotificationAsync(Order order);
}

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly INotificationTemplateService _templateService;
    private readonly INotificationHistoryService _historyService;
    private readonly AppDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IEmailService emailService,
        ISmsService smsService,
        INotificationTemplateService templateService,
        INotificationHistoryService historyService,
        AppDbContext context,
        ILogger<NotificationService> logger)
    {
        _emailService = emailService;
        _smsService = smsService;
        _templateService = templateService;
        _historyService = historyService;
        _context = context;
        _logger = logger;
    }

    private async Task<string> RenderTemplateAsync(string code, Dictionary<string, string> parameters)
    {
        var template = await _templateService.GetTemplateByCodeAsync(code);
        if (template == null)
            throw new InvalidOperationException($"模板 {code} 不存在或未启用");

        var content = template.Content;
        foreach (var param in parameters)
        {
            content = content.Replace($"{{{param.Key}}}", param.Value);
        }

        return content;
    }

    private async Task<NotificationHistory> CreateHistoryAsync(
        int userId,
        int? orderId,
        NotificationType type,
        string templateCode,
        string content,
        string? recipient)
    {
        var history = new NotificationHistory
        {
            UserId = userId,
            OrderId = orderId,
            Type = type,
            TemplateCode = templateCode,
            Content = content,
            Recipient = recipient,
            Status = NotificationStatus.Pending
        };

        await _historyService.CreateHistoryAsync(history);
        return history;
    }

    private async Task SendEmailNotificationAsync(string email, Order order)
    {
        var history = await CreateHistoryAsync(
            order.UserId,
            order.Id,
            NotificationType.Email,
            "ORDER_DELIVERY_EMAIL",
            "",
            email);

        try
        {
            var parameters = new Dictionary<string, string>
            {
                { "OrderNumber", order.OrderNumber },
                { "ProductName", order.Product.Name },
                { "DeliveryMethod", order.DeliveryMethod! },
                { "DeliveryContent", order.DeliveryContent! },
                { "DeliveryTime", order.DeliveredAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "" }
            };

            var subject = $"您的订单 {order.OrderNumber} 已发货";
            var body = await RenderTemplateAsync("ORDER_DELIVERY_EMAIL", parameters);
            history.Content = body;

            await _emailService.SendEmailAsync(email, subject, body);
            await _historyService.UpdateHistoryStatusAsync(history.Id, NotificationStatus.Success);
        }
        catch (Exception ex)
        {
            await _historyService.UpdateHistoryStatusAsync(history.Id, NotificationStatus.Failed, ex.Message);
            _logger.LogError(ex, "发送邮件通知到 {Email} 时发生错误", email);
            throw;
        }
    }

    private async Task SendSmsNotificationAsync(string phoneNumber, Order order)
    {
        var history = await CreateHistoryAsync(
            order.UserId,
            order.Id,
            NotificationType.Sms,
            "ORDER_DELIVERY_SMS",
            "",
            phoneNumber);

        try
        {
            var parameters = new Dictionary<string, string>
            {
                { "OrderNumber", order.OrderNumber },
                { "ProductName", order.Product.Name },
                { "DeliveryMethod", order.DeliveryMethod! },
                { "DeliveryContent", order.DeliveryContent! }
            };

            var content = await RenderTemplateAsync("ORDER_DELIVERY_SMS", parameters);
            history.Content = content;

            var templateCode = "SMS_123456789";
            await _smsService.SendSmsAsync(phoneNumber, templateCode, parameters);
            await _historyService.UpdateHistoryStatusAsync(history.Id, NotificationStatus.Success);
        }
        catch (Exception ex)
        {
            await _historyService.UpdateHistoryStatusAsync(history.Id, NotificationStatus.Failed, ex.Message);
            _logger.LogError(ex, "发送短信通知到 {PhoneNumber} 时发生错误", phoneNumber);
            throw;
        }
    }

    public async Task SendDeliveryNotificationAsync(Order order)
    {
        try
        {
            var user = await _context.Users.FindAsync(order.UserId);
            if (user == null)
            {
                _logger.LogWarning("用户 {UserId} 不存在，无法发送发货通知", order.UserId);
                return;
            }

            var tasks = new List<Task>();

            // 发送邮件通知
            if (user.Email != null)
            {
                var emailTask = SendEmailNotificationAsync(user.Email, order);
                tasks.Add(emailTask);
            }

            // 发送短信通知
            if (user.PhoneNumber != null)
            {
                var smsTask = SendSmsNotificationAsync(user.PhoneNumber, order);
                tasks.Add(smsTask);
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送订单 {OrderNumber} 的通知时发生错误", order.OrderNumber);
            throw;
        }
    }
} 