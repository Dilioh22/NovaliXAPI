namespace RestaurantPOS.Application.DTOs.Orders;

public class CancelOrderDto
{
    public string Reason { get; set; } = string.Empty;
}

public class UpdateOrderDto
{
    public string? Notes { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TipAmount { get; set; }
}

public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
}
