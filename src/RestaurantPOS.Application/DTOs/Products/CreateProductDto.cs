namespace RestaurantPOS.Application.DTOs.Products;

public class CreateProductDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int? PreparationTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int DisplayOrder { get; set; }
    public List<CreateProductModifierDto> Modifiers { get; set; } = new();
}

public class CreateProductModifierDto
{
    public string Name { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
}
