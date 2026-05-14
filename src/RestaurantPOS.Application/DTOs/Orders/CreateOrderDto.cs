namespace RestaurantPOS.Application.DTOs.Orders;

public class CreateOrderDto
{
    public int? TableId { get; set; }
    public string OrderType { get; set; } = "DineIn";
    public string? Notes { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public List<AddOrderItemDto> Items { get; set; } = new();
}
