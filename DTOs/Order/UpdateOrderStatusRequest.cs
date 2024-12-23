namespace XianYu.API.DTOs.Order;

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = null!;
    public string? DeliveryMethod { get; set; }
    public string? DeliveryContent { get; set; }
} 