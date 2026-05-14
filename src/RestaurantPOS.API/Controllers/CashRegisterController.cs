using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.CashRegister;

namespace RestaurantPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Cajero")]
public class CashRegisterController : ControllerBase
{
    private readonly ICashRegisterService _cashRegisterService;

    public CashRegisterController(ICashRegisterService cashRegisterService)
    {
        _cashRegisterService = cashRegisterService;
    }

    /// <summary>Obtiene todas las cajas registradoras.</summary>
    [HttpGet]
    public async Task<ActionResult<List<CashRegisterResponseDto>>> GetAll()
    {
        var registers = await _cashRegisterService.GetAllCashRegistersAsync();
        return Ok(registers);
    }

    /// <summary>Obtiene la sesión activa del usuario actual.</summary>
    [HttpGet("my-session")]
    public async Task<ActionResult<CashSessionResponseDto?>> GetMySession()
    {
        var session = await _cashRegisterService.GetMyActiveSessionAsync();
        if (session == null) return Ok(null);
        return Ok(session);
    }

    /// <summary>Abre una sesión buscando o creando la caja por nombre.</summary>
    [HttpPost("sessions/open-by-name")]
    public async Task<ActionResult<CashSessionResponseDto>> OpenSessionByName([FromBody] OpenSessionByNameDto dto)
    {
        var session = await _cashRegisterService.OpenSessionByNameAsync(dto.CashRegisterName, dto.OpeningAmount);
        return CreatedAtAction(nameof(GetSession), new { sessionId = session.Id }, session);
    }

    /// <summary>Obtiene la sesión activa de una caja.</summary>
    [HttpGet("{cashRegisterId:int}/active-session")]
    public async Task<ActionResult<CashSessionResponseDto?>> GetActiveSession(int cashRegisterId)
    {
        var session = await _cashRegisterService.GetActiveSessionAsync(cashRegisterId);
        if (session == null) return NotFound(new { message = "No hay sesión activa para esta caja." });
        return Ok(session);
    }

    /// <summary>Obtiene el historial de sesiones de una caja.</summary>
    [HttpGet("{cashRegisterId:int}/sessions")]
    public async Task<ActionResult<List<CashSessionResponseDto>>> GetSessionHistory(
        int cashRegisterId, [FromQuery] int take = 10)
    {
        var sessions = await _cashRegisterService.GetSessionHistoryAsync(cashRegisterId, take);
        return Ok(sessions);
    }

    /// <summary>Obtiene una sesión por ID.</summary>
    [HttpGet("sessions/{sessionId:int}")]
    public async Task<ActionResult<CashSessionResponseDto>> GetSession(int sessionId)
    {
        var session = await _cashRegisterService.GetSessionByIdAsync(sessionId);
        return Ok(session);
    }

    /// <summary>Abre una nueva sesión de caja.</summary>
    [HttpPost("sessions/open")]
    public async Task<ActionResult<CashSessionResponseDto>> OpenSession([FromBody] OpenSessionDto dto)
    {
        var session = await _cashRegisterService.OpenSessionAsync(dto);
        return CreatedAtAction(nameof(GetSession), new { sessionId = session.Id }, session);
    }

    /// <summary>Cierra una sesión de caja.</summary>
    [HttpPost("sessions/{sessionId:int}/close")]
    public async Task<ActionResult<CashSessionResponseDto>> CloseSession(
        int sessionId, [FromBody] CloseSessionDto dto)
    {
        var session = await _cashRegisterService.CloseSessionAsync(sessionId, dto);
        return Ok(session);
    }
}

public record OpenSessionByNameDto(string CashRegisterName, decimal OpeningAmount);
