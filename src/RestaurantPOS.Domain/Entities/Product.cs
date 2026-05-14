using System.Collections.Generic;

namespace RestaurantPOS.Domain.Entities;

public class Product : BaseEntity
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int? PreparationTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int DisplayOrder { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<ProductModifier> Modifiers { get; set; } = new List<ProductModifier>();
}
