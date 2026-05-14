namespace RestaurantPOS.Domain.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? UserRole { get; }
    string? Email { get; }
}
