using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantPOS.Application.DTOs.Auth;

namespace RestaurantPOS.Application.Common.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginDto dto);
    Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
    Task RevokeTokenAsync(int userId);
    Task<UserResponseDto> GetCurrentUserAsync(int userId);
    Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
    Task<List<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto> UpdateUserAsync(int userId, CreateUserDto dto);
    Task DeactivateUserAsync(int userId);
    Task<UserResponseDto> GetUserByIdAsync(int userId);
    Task<UserResponseDto> ToggleUserStatusAsync(int userId, bool isActive);
    Task ResetPasswordAsync(int userId, string newPassword);
}
