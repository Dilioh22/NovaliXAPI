namespace RestaurantPOS.Application.DTOs.Orders;

public class AddOrderItemsDto
{
    public List<AddOrderItemDto> Items { get; set; } = new();
}
