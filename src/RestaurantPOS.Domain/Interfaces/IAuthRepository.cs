using System.Threading.Tasks;
using RestaurantPOS.Domain.Entities;

namespace RestaurantPOS.Domain.Interfaces;

public interface IAuthRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
}
