using Microsoft.AspNetCore.Http;
using RestaurantPOS.Domain.Interfaces;
using System.Security.Claims;

namespace RestaurantPOS.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public string? UserRole =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

    public string? Email =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
}
