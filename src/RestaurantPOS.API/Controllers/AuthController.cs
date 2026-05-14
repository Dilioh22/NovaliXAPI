using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Auth;
using RestaurantPOS.Domain.Interfaces;

namespace RestaurantPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUser;

    public AuthController(IAuthService authService, ICurrentUserService currentUser)
    {
        _authService = authService;
        _currentUser = currentUser;
    }

    /// <summary>Inicia sesión y devuelve tokens JWT.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    /// <summary>Refresca el access token usando el refresh token.</summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto);
        return Ok(result);
    }

    /// <summary>Revoca el refresh token del usuario autenticado.</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = _currentUser.UserId!.Value;
        await _authService.RevokeTokenAsync(userId);
        return NoContent();
    }

    /// <summary>Obtiene información del usuario autenticado.</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
    {
        var result = await _authService.GetCurrentUserAsync(_currentUser.UserId!.Value);
        return Ok(result);
    }
}
