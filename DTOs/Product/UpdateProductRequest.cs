namespace XianYu.API.DTOs.Product;

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
} 