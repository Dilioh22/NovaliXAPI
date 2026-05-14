namespace RestaurantPOS.Domain.Entities;

public class ProductModifier : BaseEntity
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
    public Product Product { get; set; } = null!;
}
