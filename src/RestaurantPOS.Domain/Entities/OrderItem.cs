using System;
using System.Collections.Generic;

namespace RestaurantPOS.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public string Status { get; set; } = "Pendiente";
    public string? Notes { get; set; }
    public DateTime? SentToKitchenAt { get; set; }
    public DateTime? ReadyAt { get; set; }
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ICollection<OrderItemModifier> Modifiers { get; set; } = new List<OrderItemModifier>();
}
