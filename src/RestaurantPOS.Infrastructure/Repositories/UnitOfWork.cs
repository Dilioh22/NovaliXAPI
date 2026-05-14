using System;
using System.Threading;
using System.Threading.Tasks;
using RestaurantPOS.Domain.Interfaces;
using RestaurantPOS.Infrastructure.Data;

namespace RestaurantPOS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
