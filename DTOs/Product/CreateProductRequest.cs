namespace XianYu.API.DTOs.Product;

public class CreateProductRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;
} 