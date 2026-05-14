using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Interfaces;
using RestaurantPOS.Infrastructure.Data;

namespace RestaurantPOS.Infrastructure.Repositories;

public class AuthRepository : GenericRepository<User>, IAuthRepository
{
    public AuthRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
        await _dbSet.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.IsActive);
}
