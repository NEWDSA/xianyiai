using Microsoft.EntityFrameworkCore;
using XianYu.API.DTOs.Order;
using XianYu.API.Infrastructure.Data;
using XianYu.API.Models;

namespace XianYu.API.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetOrdersAsync();
    Task<OrderDto?> GetOrderAsync(int id);
    Task<OrderDto?> GetOrderByNumberAsync(string orderNumber);
    Task<OrderDto> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderDto?> UpdateOrderStatusAsync(int id, UpdateOrderStatusRequest request);
    Task<OrderDto?> DeliverOrderAsync(int id);
}

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        AppDbContext context,
        INotificationService notificationService,
        ILogger<OrderService> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Product)
            .Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                UserId = o.UserId,
                Username = o.User.Username,
                ProductId = o.ProductId,
                ProductName = o.Product.Name,
                Price = o.Price,
                Quantity = o.Quantity,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                DeliveryMethod = o.DeliveryMethod,
                DeliveryContent = o.DeliveryContent,
                CreatedAt = o.CreatedAt,
                PaidAt = o.PaidAt,
                DeliveredAt = o.DeliveredAt
            })
            .ToListAsync();

        return orders;
    }

    public async Task<OrderDto?> GetOrderAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Product)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return null;

        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            UserId = order.UserId,
            Username = order.User.Username,
            ProductId = order.ProductId,
            ProductName = order.Product.Name,
            Price = order.Price,
            Quantity = order.Quantity,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            DeliveryMethod = order.DeliveryMethod,
            DeliveryContent = order.DeliveryContent,
            CreatedAt = order.CreatedAt,
            PaidAt = order.PaidAt,
            DeliveredAt = order.DeliveredAt
        };
    }

    public async Task<OrderDto?> GetOrderByNumberAsync(string orderNumber)
    {
        var order = await _context.Orders
            .Include(o => o.Product)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        if (order == null)
            return null;

        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            UserId = order.UserId,
            Username = order.User.Username,
            ProductId = order.ProductId,
            ProductName = order.Product.Name,
            Price = order.Price,
            Quantity = order.Quantity,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            DeliveryMethod = order.DeliveryMethod,
            DeliveryContent = order.DeliveryContent,
            CreatedAt = order.CreatedAt,
            PaidAt = order.PaidAt,
            DeliveredAt = order.DeliveredAt
        };
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request)
    {
        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null)
            throw new InvalidOperationException("商品不存在");

        if (!product.IsActive)
            throw new InvalidOperationException("商品已下架");

        if (product.Stock < request.Quantity)
            throw new InvalidOperationException("商品库存不足");

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            UserId = request.UserId,
            ProductId = request.ProductId,
            Price = product.Price,
            Quantity = request.Quantity,
            TotalAmount = product.Price * request.Quantity,
            Status = OrderStatus.Created,
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);

        // 减少库存
        product.Stock -= request.Quantity;
        if (product.Stock == 0)
            product.IsActive = false;

        await _context.SaveChangesAsync();

        return await GetOrderAsync(order.Id) ?? 
            throw new InvalidOperationException("Failed to create order");
    }

    public async Task<OrderDto?> UpdateOrderStatusAsync(int id, UpdateOrderStatusRequest request)
    {
        var order = await _context.Orders
            .Include(o => o.Product)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return null;

        var oldStatus = order.Status;
        order.Status = Enum.Parse<OrderStatus>(request.Status);

        // 更新相关时间戳
        switch (order.Status)
        {
            case OrderStatus.Paid:
                order.PaidAt = DateTime.UtcNow;
                break;
            case OrderStatus.Delivered:
                order.DeliveredAt = DateTime.UtcNow;
                break;
        }

        await _context.SaveChangesAsync();

        // 如果订单状态从其他状态变为已支付，触发自动发货
        if (oldStatus != OrderStatus.Paid && order.Status == OrderStatus.Paid)
        {
            if (order.Product.Type == ProductType.Virtual)
            {
                await DeliverOrderAsync(order.Id);
            }
        }

        return await GetOrderAsync(order.Id);
    }

    public async Task<OrderDto?> DeliverOrderAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Product)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return null;

        if (order.Status != OrderStatus.Paid)
            throw new InvalidOperationException("只有已支付的订单才能发货");

        // 根据商品类型设置发货方式和内容
        if (order.Product.Type == ProductType.Virtual)
        {
            order.DeliveryMethod = "自动发货";
            order.DeliveryContent = GenerateDeliveryContent();
        }
        else
        {
            order.DeliveryMethod = "人工发货";
            order.DeliveryContent = "请等待客服人员处理";
        }

        order.Status = OrderStatus.Delivered;
        order.DeliveredAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // 发送通知
        try
        {
            await _notificationService.SendDeliveryNotificationAsync(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送发货通知失败");
            // 不影响发货流程，继续执行
        }

        return await GetOrderAsync(order.Id);
    }

    private string GenerateOrderNumber()
    {
        return $"XY{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }

    private string GenerateDeliveryContent()
    {
        // 这里应该根据实际业务生成发货内容
        // 例如：生成激活码、卡密等
        return $"测试卡密：{Guid.NewGuid():N}";
    }
} 