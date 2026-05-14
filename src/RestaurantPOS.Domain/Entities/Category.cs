using System.Collections.Generic;

namespace RestaurantPOS.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public string? IconName { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
