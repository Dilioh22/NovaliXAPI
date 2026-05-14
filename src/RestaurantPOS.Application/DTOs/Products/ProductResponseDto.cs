namespace RestaurantPOS.Application.DTOs.Products;

public class ProductResponseDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int? PreparationTime { get; set; }
    public bool IsAvailable { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public List<ProductModifierDto> Modifiers { get; set; } = new();
}

public class ProductModifierDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
}
