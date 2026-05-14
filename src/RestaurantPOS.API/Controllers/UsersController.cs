using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Auth;

namespace RestaurantPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Lista todos los usuarios del sistema.</summary>
    [HttpGet]
    public async Task<ActionResult<List<UserResponseDto>>> GetAll()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>Obtiene un usuario por ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> GetById(int id)
    {
        var user = await _authService.GetUserByIdAsync(id);
        return Ok(user);
    }

    /// <summary>Crea un nuevo usuario.</summary>
    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] CreateUserDto dto)
    {
        var user = await _authService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>Actualiza un usuario existente.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> Update(int id, [FromBody] CreateUserDto dto)
    {
        var user = await _authService.UpdateUserAsync(id, dto);
        return Ok(user);
    }

    /// <summary>Activa o desactiva un usuario.</summary>
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<UserResponseDto>> ToggleStatus(int id, [FromBody] ToggleUserStatusDto dto)
    {
        var user = await _authService.ToggleUserStatusAsync(id, dto.IsActive);
        return Ok(user);
    }

    /// <summary>Restablece la contraseña de un usuario.</summary>
    [HttpPost("{id:int}/reset-password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
    {
        await _authService.ResetPasswordAsync(id, dto.NewPassword);
        return NoContent();
    }

    /// <summary>Desactiva un usuario.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _authService.DeactivateUserAsync(id);
        return NoContent();
    }
}

public record ToggleUserStatusDto(bool IsActive);
public record ResetPasswordDto(string NewPassword);
