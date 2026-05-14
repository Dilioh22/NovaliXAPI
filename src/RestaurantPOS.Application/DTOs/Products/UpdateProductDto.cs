namespace RestaurantPOS.Application.DTOs.Products;

public class UpdateProductDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int? PreparationTime { get; set; }
    public bool IsAvailable { get; set; }
    public int DisplayOrder { get; set; }
}
