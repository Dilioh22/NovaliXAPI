using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Infrastructure.Data;

namespace RestaurantPOS.Infrastructure.Services;

/// <summary>
/// Genera números de orden sin race condition.
/// El SemaphoreSlim(1,1) serializa el acceso: solo un hilo a la vez
/// puede leer el conteo y generar el número, eliminando duplicados
/// en escenarios de múltiples cajeros simultáneos en la misma instancia.
/// </summary>
public sealed class OrderNumberService : IOrderNumberService, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public OrderNumberService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<string> GenerateAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            // Crear scope propio porque este servicio es Singleton
            // pero AppDbContext es Scoped
            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var today = DateTime.UtcNow.Date;
            var count = await context.Orders.CountAsync(o => o.CreatedAt.Date == today);
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D4}";
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose() => _semaphore.Dispose();
}
