using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Reservations;

namespace RestaurantPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _service;

    public ReservationsController(IReservationService service)
    {
        _service = service;
    }

    /// <summary>Lista todas las reservas. Filtra por fecha (YYYY-MM-DD) y/o estado.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ReservationResponseDto>>> GetAll(
        [FromQuery] DateTime? date,
        [FromQuery] string? status)
    {
        return Ok(await _service.GetAllAsync(date, status));
    }

    /// <summary>Reservas de una mesa específica.</summary>
    [HttpGet("table/{tableId:int}")]
    public async Task<ActionResult<List<ReservationResponseDto>>> GetByTable(int tableId)
    {
        return Ok(await _service.GetByTableAsync(tableId));
    }

    /// <summary>Obtiene una reserva por ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservationResponseDto>> GetById(int id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    /// <summary>Crea una nueva reserva.</summary>
    [HttpPost]
    public async Task<ActionResult<ReservationResponseDto>> Create([FromBody] CreateReservationDto dto)
    {
        var reservation = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }

    /// <summary>Actualiza los datos de una reserva.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReservationResponseDto>> Update(int id, [FromBody] UpdateReservationDto dto)
    {
        return Ok(await _service.UpdateAsync(id, dto));
    }

    /// <summary>Cambia el estado de una reserva (Pendiente, Confirmada, Cancelada, NoShow, Completada).</summary>
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ReservationResponseDto>> UpdateStatus(int id, [FromBody] UpdateReservationStatusDto dto)
    {
        return Ok(await _service.UpdateStatusAsync(id, dto));
    }

    /// <summary>Elimina (desactiva) una reserva.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
