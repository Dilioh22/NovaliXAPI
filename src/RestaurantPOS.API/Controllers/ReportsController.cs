using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Reports;

namespace RestaurantPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Cajero")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>Reporte de ventas por rango de fechas.</summary>
    [HttpGet("sales")]
    public async Task<ActionResult<SalesReportDto>> GetSalesReport(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var report = await _reportService.GetSalesReportAsync(from, to);
        return Ok(report);
    }

    /// <summary>Reporte de ventas del día.</summary>
    [HttpGet("sales/today")]
    public async Task<ActionResult<SalesReportDto>> GetTodayReport()
    {
        var report = await _reportService.GetDailySalesReportAsync(DateTime.UtcNow.Date);
        return Ok(report);
    }

    /// <summary>Reporte de productos más vendidos.</summary>
    [HttpGet("top-products")]
    public async Task<ActionResult<List<ProductSalesDto>>> GetTopProducts(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int top = 10)
    {
        var products = await _reportService.GetTopProductsAsync(from, to, top);
        return Ok(products);
    }
}
