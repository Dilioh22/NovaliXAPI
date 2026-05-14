using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RestaurantPOS.API.Hubs;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Payments;

namespace RestaurantPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IHubContext<OrderHub> _hubContext;

    public PaymentsController(IPaymentService paymentService, IHubContext<OrderHub> hubContext)
    {
        _paymentService = paymentService;
        _hubContext = hubContext;
    }

    /// <summary>Obtiene los métodos de pago disponibles.</summary>
    [HttpGet("methods")]
    public async Task<ActionResult<List<PaymentMethodDto>>> GetMethods()
    {
        var methods = await _paymentService.GetPaymentMethodsAsync();
        return Ok(methods);
    }

    /// <summary>Obtiene los pagos de una orden.</summary>
    [HttpGet("by-order/{orderId:int}")]
    public async Task<ActionResult<List<PaymentResponseDto>>> GetByOrder(int orderId)
    {
        var payments = await _paymentService.GetPaymentsByOrderAsync(orderId);
        return Ok(payments);
    }

    /// <summary>Procesa un pago para una orden.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Cajero")]
    public async Task<ActionResult<PaymentResponseDto>> Process([FromBody] ProcessPaymentDto dto)
    {
        var payment = await _paymentService.ProcessPaymentAsync(dto);
        await _hubContext.Clients.Group("cashier").SendAsync("PaymentProcessed", payment);
        if (payment.IsOrderFullyPaid)
            await _hubContext.Clients.Group("kitchen").SendAsync("OrderPaid", new { orderId = payment.OrderId });
        return CreatedAtAction(nameof(GetByOrder), new { orderId = payment.OrderId }, payment);
    }
}
