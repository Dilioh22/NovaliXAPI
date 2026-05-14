using System;
using System.Collections.Generic;

namespace RestaurantPOS.Application.DTOs.Orders;

public class OrderResponseDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int? TableId { get; set; }
    public int? TableNumber { get; set; }
    public string? ZoneName { get; set; }
    public int WaiterId { get; set; }
    public string WaiterName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal Total { get; set; }
    public decimal TipAmount { get; set; }
    public string? Notes { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime? SentToKitchenAt { get; set; }
    public DateTime? ReadyAt { get; set; }
    public List<OrderItemModifierDto> Modifiers { get; set; } = new();
}

public class OrderItemModifierDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
}
