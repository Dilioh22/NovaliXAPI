using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.Common.Mappings;
using RestaurantPOS.Application.Services;
using RestaurantPOS.Application.Validators;

namespace RestaurantPOS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        services.AddValidatorsFromAssemblyContaining<LoginValidator>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ITableService, TableService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<ICashRegisterService, CashRegisterService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IReservationService, ReservationService>();

        return services;
    }
}
