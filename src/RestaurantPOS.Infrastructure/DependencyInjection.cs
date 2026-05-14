using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Domain.Interfaces;
using RestaurantPOS.Infrastructure.Data;
using RestaurantPOS.Infrastructure.Repositories;
using RestaurantPOS.Infrastructure.Services;

namespace RestaurantPOS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var serverVersion = new MySqlServerVersion(new Version(9, 3, 0));
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString,
                serverVersion,
                mysql => mysql.MigrationsAssembly("RestaurantPOS.Infrastructure")));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IAuthRepository, AuthRepository>();

        services.AddSingleton<IOrderNumberService, OrderNumberService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
