using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RestaurantPOS.API.Hubs;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Orders;

namespace RestaurantPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IHubContext<OrderHub> _hubContext;

    public OrdersController(IOrderService orderService, IHubContext<OrderHub> hubContext)
    {
        _orderService = orderService;
        _hubContext = hubContext;
    }

    /// <summary>Obtiene todas las órdenes, opcionalmente filtradas por estado.</summary>
    [HttpGet]
    public async Task<ActionResult<List<OrderResponseDto>>> GetAll([FromQuery] string? status = null)
    {
        var orders = await _orderService.GetAllOrdersAsync(status);
        return Ok(orders);
    }

    /// <summary>Obtiene las órdenes activas (no pagadas ni canceladas).</summary>
    [HttpGet("active")]
    public async Task<ActionResult<List<OrderResponseDto>>> GetActive()
    {
        var orders = await _orderService.GetActiveOrdersAsync();
        return Ok(orders);
    }

    /// <summary>Obtiene las órdenes activas para la pantalla de cocina.</summary>
    [HttpGet("kitchen")]
    public async Task<ActionResult<List<OrderResponseDto>>> GetKitchenOrders()
    {
        var orders = await _orderService.GetActiveOrdersForKitchenAsync();
        return Ok(orders);
    }

    /// <summary>Obtiene una orden por ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        return Ok(order);
    }

    /// <summary>Obtiene la orden activa de una mesa.</summary>
    [HttpGet("by-table/{tableId:int}")]
    public async Task<ActionResult<OrderResponseDto>> GetByTable(int tableId)
    {
        var order = await _orderService.GetOrderByTableAsync(tableId);
        return Ok(order);
    }

    /// <summary>Crea una nueva orden.</summary>
    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Create([FromBody] CreateOrderDto dto)
    {
        var order = await _orderService.CreateOrderAsync(dto);
        await _hubContext.Clients.Group("kitchen").SendAsync("NewOrder", order);
        await _hubContext.Clients.Group("cashier").SendAsync("OrderCreated", order);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>Agrega uno o varios items a una orden existente.</summary>
    [HttpPost("{id:int}/items")]
    public async Task<ActionResult<OrderResponseDto>> AddItems(int id, [FromBody] AddOrderItemsDto dto)
    {
        var order = await _orderService.AddItemsToOrderAsync(id, dto);
        await _hubContext.Clients.Group("kitchen").SendAsync("OrderUpdated", order);
        return Ok(order);
    }

    /// <summary>Actualiza información de la orden (descuentos, notas, propina).</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrderResponseDto>> Update(int id, [FromBody] UpdateOrderDto dto)
    {
        var order = await _orderService.UpdateOrderAsync(id, dto);
        return Ok(order);
    }

    /// <summary>Actualiza el estado de la orden.</summary>
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<OrderResponseDto>> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        var order = await _orderService.UpdateOrderStatusAsync(id, dto);
        await _hubContext.Clients.Group("kitchen").SendAsync("OrderStatusChanged", order);
        await _hubContext.Clients.Group("cashier").SendAsync("OrderStatusChanged", order);
        return Ok(order);
    }

    /// <summary>Elimina un item de la orden.</summary>
    [HttpDelete("{id:int}/items/{itemId:int}")]
    public async Task<IActionResult> RemoveItem(int id, int itemId)
    {
        await _orderService.RemoveOrderItemAsync(id, itemId);
        return NoContent();
    }

    /// <summary>Envía la orden a cocina.</summary>
    [HttpPost("{id:int}/send-to-kitchen")]
    public async Task<ActionResult<OrderResponseDto>> SendToKitchen(int id)
    {
        var order = await _orderService.SendToKitchenAsync(id);
        await _hubContext.Clients.Group("kitchen").SendAsync("OrderSentToKitchen", order);
        return Ok(order);
    }

    /// <summary>Cancela una orden.</summary>
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelOrderDto dto)
    {
        await _orderService.CancelOrderAsync(id, dto.Reason);
        await _hubContext.Clients.Group("kitchen").SendAsync("OrderCancelled", new { id });
        await _hubContext.Clients.Group("cashier").SendAsync("OrderCancelled", new { id });
        return NoContent();
    }

    /// <summary>Marca un item de la orden como listo.</summary>
    [HttpPatch("{id:int}/items/{itemId:int}/ready")]
    public async Task<IActionResult> MarkItemReady(int id, int itemId)
    {
        await _orderService.MarkOrderItemReadyAsync(id, itemId);
        return NoContent();
    }

    /// <summary>Marca toda la orden como lista.</summary>
    [HttpPatch("{id:int}/ready")]
    public async Task<ActionResult<OrderResponseDto>> MarkReady(int id)
    {
        var order = await _orderService.MarkOrderReadyAsync(id);
        await _hubContext.Clients.Group("cashier").SendAsync("OrderReady", order);
        await _hubContext.Clients.Group("kitchen").SendAsync("OrderReady", new { orderId = order.Id });
        return Ok(order);
    }
}
