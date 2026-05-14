using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Tables;

namespace RestaurantPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TablesController : ControllerBase
{
    private readonly ITableService _tableService;

    public TablesController(ITableService tableService)
    {
        _tableService = tableService;
    }

    /// <summary>Obtiene todas las zonas con sus mesas.</summary>
    [HttpGet("zones")]
    public async Task<ActionResult<List<TableZoneResponseDto>>> GetZonesWithTables()
    {
        var zones = await _tableService.GetAllZonesWithTablesAsync();
        return Ok(zones);
    }

    /// <summary>Obtiene solo las zonas (sin mesas).</summary>
    [HttpGet("zones/list")]
    public async Task<ActionResult<List<TableZoneResponseDto>>> GetZones()
    {
        var zones = await _tableService.GetAllZonesAsync();
        return Ok(zones);
    }

    /// <summary>Crea una nueva zona.</summary>
    [HttpPost("zones")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TableZoneResponseDto>> CreateZone([FromBody] CreateTableZoneDto dto)
    {
        var zone = await _tableService.CreateZoneAsync(dto);
        return CreatedAtAction(nameof(GetZonesWithTables), zone);
    }

    /// <summary>Obtiene todas las mesas.</summary>
    [HttpGet]
    public async Task<ActionResult<List<TableResponseDto>>> GetAll()
    {
        var tables = await _tableService.GetAllTablesAsync();
        return Ok(tables);
    }

    /// <summary>Obtiene una mesa por ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TableResponseDto>> GetById(int id)
    {
        var table = await _tableService.GetTableByIdAsync(id);
        return Ok(table);
    }

    /// <summary>Crea una nueva mesa.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TableResponseDto>> Create([FromBody] CreateTableDto dto)
    {
        var table = await _tableService.CreateTableAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = table.Id }, table);
    }

    /// <summary>Actualiza los datos de una mesa (número, capacidad, zona, etc.).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TableResponseDto>> Update(int id, [FromBody] UpdateTableDto dto)
    {
        var table = await _tableService.UpdateTableAsync(id, dto);
        return Ok(table);
    }

    /// <summary>Actualiza el estado de una mesa.</summary>
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<TableResponseDto>> UpdateStatus(int id, [FromBody] UpdateTableStatusDto dto)
    {
        var table = await _tableService.UpdateTableStatusAsync(id, dto);
        return Ok(table);
    }

    /// <summary>Elimina (desactiva) una mesa.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _tableService.DeleteTableAsync(id);
        return NoContent();
    }
}
