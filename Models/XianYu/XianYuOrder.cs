namespace XianYu.API.Models.XianYu;

public class XianYuOrder
{
    public string OrderId { get; set; } = string.Empty;
    public string BuyerId { get; set; } = string.Empty;
    public string BuyerName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreateTime { get; set; }
    public DateTime? PayTime { get; set; }
    public string DeliveryMethod { get; set; } = string.Empty;
    public string DeliveryContent { get; set; } = string.Empty;
    public bool IsAutoDelivery { get; set; }
    public DateTime? DeliveryTime { get; set; }
    public string Remarks { get; set; } = string.Empty;
}
