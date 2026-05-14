namespace RestaurantPOS.Application.DTOs.Orders;

public class AddOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public string? Notes { get; set; }
    public List<int> ModifierIds { get; set; } = new();
}
