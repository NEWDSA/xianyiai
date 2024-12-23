using Microsoft.EntityFrameworkCore;
using XianYu.API.Infrastructure.Data;
using XianYu.API.Models;
using XianYu.API.Services;

namespace XianYu.API.Infrastructure.BackgroundServices;

public class AutoDeliveryBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AutoDeliveryBackgroundService> _logger;
    private readonly IConfiguration _configuration;

    public AutoDeliveryBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<AutoDeliveryBackgroundService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("自动发货服务已启动");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingOrdersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理待发货订单时发生错误");
            }

            try
            {
                // 等待指定时间后再次检查
                var checkInterval = _configuration.GetValue("AutoDelivery:CheckInterval", 30);
                _logger.LogDebug("等待 {Seconds} 秒后继续处理", checkInterval);
                await Task.Delay(TimeSpan.FromSeconds(checkInterval), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("自动发货服务正在停止");
                break;
            }
        }

        _logger.LogInformation("自动发货服务已停止");
    }

    private async Task ProcessPendingOrdersAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("开始处理待发货订单");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

            var batchSize = _configuration.GetValue("AutoDelivery:BatchSize", 10);
            _logger.LogDebug("批量处理大小: {BatchSize}", batchSize);

            // 获取待发货的订单
            var pendingOrders = await context.Orders
                .Include(o => o.Product)
                .Include(o => o.User)
                .Where(o => o.Status == OrderStatus.Paid && o.Product.Type == ProductType.Virtual)
                .Take(batchSize)
                .ToListAsync(stoppingToken);

            _logger.LogInformation("找到 {Count} 个待发货订单", pendingOrders.Count);

            foreach (var order in pendingOrders)
            {
                try
                {
                    _logger.LogInformation(
                        "正在处理订单 {OrderNumber} 的自动发货 (商品: {ProductName}, 用户: {Username})", 
                        order.OrderNumber,
                        order.Product.Name,
                        order.User.Username);

                    await orderService.DeliverOrderAsync(order.Id);

                    _logger.LogInformation(
                        "订单 {OrderNumber} 自动发货成功", 
                        order.OrderNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "处理订单 {OrderNumber} 的自动发货时发生错误",
                        order.OrderNumber);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理待发货订单批次时发生错误");
            throw;
        }
    }
} 