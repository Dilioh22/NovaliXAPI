using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Invoices;

namespace RestaurantPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Cajero")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    /// <summary>Genera una factura para una orden pagada.</summary>
    [HttpPost]
    public async Task<ActionResult<InvoiceResponseDto>> Generate([FromBody] GenerateInvoiceDto dto)
    {
        var invoice = await _invoiceService.GenerateInvoiceAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
    }

    /// <summary>Obtiene una factura por ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvoiceResponseDto>> GetById(int id)
    {
        var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
        return Ok(invoice);
    }

    /// <summary>Obtiene la factura de una orden.</summary>
    [HttpGet("by-order/{orderId:int}")]
    public async Task<ActionResult<InvoiceResponseDto>> GetByOrder(int orderId)
    {
        var invoice = await _invoiceService.GetInvoiceByOrderAsync(orderId);
        return Ok(invoice);
    }

    /// <summary>Lista facturas en un rango de fechas.</summary>
    [HttpGet]
    public async Task<ActionResult<List<InvoiceResponseDto>>> GetAll(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var invoices = await _invoiceService.GetInvoicesAsync(from, to);
        return Ok(invoices);
    }

    /// <summary>Anula una factura.</summary>
    [HttpPost("{id:int}/void")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Void(int id, [FromBody] VoidInvoiceDto dto)
    {
        await _invoiceService.VoidInvoiceAsync(id, dto.Reason);
        return NoContent();
    }
}

public class VoidInvoiceDto
{
    public string Reason { get; set; } = string.Empty;
}
