using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestaurantPOS.Application.Common.Exceptions;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Auth;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RestaurantPOS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(IAuthRepository authRepository, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
    {
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _authRepository.GetByEmailAsync(dto.Email)
            ?? throw new BusinessRuleException("Credenciales inválidas.");

        if (!user.IsActive)
            throw new BusinessRuleException("El usuario está inactivo.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new BusinessRuleException("Credenciales inválidas.");

        user.LastLoginAt = DateTime.UtcNow;
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        _authRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return BuildTokenResponse(user, refreshToken);
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var user = await _authRepository.GetByRefreshTokenAsync(dto.RefreshToken)
            ?? throw new BusinessRuleException("Token de refresco inválido.");

        if (user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new BusinessRuleException("El token de refresco ha expirado.");

        var newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        _authRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return BuildTokenResponse(user, newRefreshToken);
    }

    public async Task RevokeTokenAsync(int userId)
    {
        var user = await _authRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        _authRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<UserResponseDto> GetCurrentUserAsync(int userId)
    {
        var user = await _authRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);
        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
    {
        var existing = await _authRepository.GetByEmailAsync(dto.Email);
        if (existing != null)
            throw new BusinessRuleException("Ya existe un usuario con ese email.");

        var user = _mapper.Map<User>(dto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        await _authRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _authRepository.GetAllAsync();
        return users.Select(u => _mapper.Map<UserResponseDto>(u)).ToList();
    }

    public async Task<UserResponseDto> UpdateUserAsync(int userId, CreateUserDto dto)
    {
        var user = await _authRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Role = dto.Role;
        user.Pin = dto.Pin;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        _authRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task DeactivateUserAsync(int userId)
    {
        var user = await _authRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        _authRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int userId)
    {
        var user = await _authRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);
        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto> ToggleUserStatusAsync(int userId, bool isActive)
    {
        var user = await _authRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;
        _authRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task ResetPasswordAsync(int userId, string newPassword)
    {
        var user = await _authRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;
        _authRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    private TokenResponseDto BuildTokenResponse(User user, string refreshToken)
    {
        var accessToken = GenerateJwtToken(user);
        var expiry = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60"));

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiry,
            User = _mapper.Map<UserResponseDto>(user)
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured.")));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
