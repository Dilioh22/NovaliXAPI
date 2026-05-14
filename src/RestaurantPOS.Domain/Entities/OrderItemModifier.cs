using System;

namespace RestaurantPOS.Domain.Entities;

public class OrderItemModifier
{
    public int Id { get; set; }
    public int OrderItemId { get; set; }
    public int ProductModifierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderItem OrderItem { get; set; } = null!;
    public ProductModifier ProductModifier { get; set; } = null!;
}
