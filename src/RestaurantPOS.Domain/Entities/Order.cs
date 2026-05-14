using System.Collections.Generic;

namespace RestaurantPOS.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public int? TableId { get; set; }
    public int WaiterId { get; set; }
    public string Status { get; set; } = "Pendiente";
    public string OrderType { get; set; } = "DineIn";
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; } = 0.15m;
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal Total { get; set; }
    public decimal TipAmount { get; set; }
    public string? Notes { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public Table? Table { get; set; }
    public User Waiter { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public Invoice? Invoice { get; set; }
}
