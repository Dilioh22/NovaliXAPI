using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Domain.Entities;

namespace RestaurantPOS.Domain.Interfaces;

public interface IAppDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductModifier> ProductModifiers { get; }
    DbSet<TableZone> TableZones { get; }
    DbSet<Table> Tables { get; }
    DbSet<User> Users { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<OrderItemModifier> OrderItemModifiers { get; }
    DbSet<PaymentMethod> PaymentMethods { get; }
    DbSet<Payment> Payments { get; }
    DbSet<CashRegister> CashRegisters { get; }
    DbSet<CashRegisterSession> CashRegisterSessions { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<Reservation> Reservations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
