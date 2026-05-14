using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantPOS.Application.DTOs.Orders;

namespace RestaurantPOS.Application.Common.Interfaces;

public interface IOrderService
{
    Task<List<OrderResponseDto>> GetAllOrdersAsync(string? status = null);
    Task<List<OrderResponseDto>> GetActiveOrdersAsync();
    Task<OrderResponseDto> GetOrderByIdAsync(int id);
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderResponseDto> AddItemToOrderAsync(int orderId, AddOrderItemDto dto);
    Task<OrderResponseDto> AddItemsToOrderAsync(int orderId, AddOrderItemsDto dto);
    Task<OrderResponseDto> UpdateOrderAsync(int orderId, UpdateOrderDto dto);
    Task<OrderResponseDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
    Task RemoveOrderItemAsync(int orderId, int itemId);
    Task<OrderResponseDto> SendToKitchenAsync(int orderId);
    Task CancelOrderAsync(int orderId, string reason);
    Task<List<OrderResponseDto>> GetActiveOrdersForKitchenAsync();
    Task<OrderResponseDto> GetOrderByTableAsync(int tableId);
    Task MarkOrderItemReadyAsync(int orderId, int itemId);
    Task<OrderResponseDto> MarkOrderReadyAsync(int orderId);
}
