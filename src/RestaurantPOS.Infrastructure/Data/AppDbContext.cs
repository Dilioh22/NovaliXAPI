using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Interfaces;

namespace RestaurantPOS.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductModifier> ProductModifiers => Set<ProductModifier>();
    public DbSet<TableZone> TableZones => Set<TableZone>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderItemModifier> OrderItemModifiers => Set<OrderItemModifier>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<CashRegister> CashRegisters => Set<CashRegister>();
    public DbSet<CashRegisterSession> CashRegisterSessions => Set<CashRegisterSession>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Category
        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.IconName).HasMaxLength(100);
        });

        // Product
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
            e.Property(x => x.ImageUrl).HasMaxLength(500);
            e.HasOne(x => x.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ProductModifier
        modelBuilder.Entity<ProductModifier>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.PriceAdjustment).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Product)
                .WithMany(p => p.Modifiers)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TableZone
        modelBuilder.Entity<TableZone>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
        });

        // Table
        modelBuilder.Entity<Table>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.Property(x => x.Shape).HasMaxLength(50);
            e.HasOne(x => x.Zone)
                .WithMany(z => z.Tables)
                .HasForeignKey(x => x.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.ZoneId, x.Number });
        });

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Role).HasMaxLength(50).IsRequired();
            e.Property(x => x.Pin).HasMaxLength(10);
            e.Property(x => x.RefreshToken).HasMaxLength(500);
            e.Ignore(x => x.FullName);
            e.HasIndex(x => x.Email).IsUnique();
        });

        // Order
        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.OrderNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.Property(x => x.OrderType).HasMaxLength(50).IsRequired();
            e.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
            e.Property(x => x.TaxRate).HasColumnType("decimal(5,4)");
            e.Property(x => x.TaxAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.DiscountPercent).HasColumnType("decimal(5,2)");
            e.Property(x => x.Total).HasColumnType("decimal(18,2)");
            e.Property(x => x.TipAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.CustomerName).HasMaxLength(200);
            e.Property(x => x.CustomerPhone).HasMaxLength(50);
            e.HasOne(x => x.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(x => x.TableId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Waiter)
                .WithMany()
                .HasForeignKey(x => x.WaiterId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.OrderNumber).IsUnique();
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasOne(x => x.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // OrderItemModifier
        modelBuilder.Entity<OrderItemModifier>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.PriceAdjustment).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.OrderItem)
                .WithMany(i => i.Modifiers)
                .HasForeignKey(x => x.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.ProductModifier)
                .WithMany()
                .HasForeignKey(x => x.ProductModifierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PaymentMethod
        modelBuilder.Entity<PaymentMethod>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Code).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Code).IsUnique();
        });

        // Payment
        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.Property(x => x.ReceivedAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.ChangeAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.Reference).HasMaxLength(200);
            e.HasOne(x => x.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.PaymentMethod)
                .WithMany()
                .HasForeignKey(x => x.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.ProcessedBy)
                .WithMany()
                .HasForeignKey(x => x.ProcessedById)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.CashSession)
                .WithMany(s => s.Payments)
                .HasForeignKey(x => x.CashSessionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CashRegister
        modelBuilder.Entity<CashRegister>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Location).HasMaxLength(200);
        });

        // CashRegisterSession
        modelBuilder.Entity<CashRegisterSession>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.OpeningAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.ClosingAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.ExpectedAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.Difference).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalCash).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalCard).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalTransfer).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalSales).HasColumnType("decimal(18,2)");
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasOne(x => x.CashRegister)
                .WithMany(cr => cr.Sessions)
                .HasForeignKey(x => x.CashRegisterId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.OpenedBy)
                .WithMany()
                .HasForeignKey(x => x.OpenedById)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.ClosedBy)
                .WithMany()
                .HasForeignKey(x => x.ClosedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Invoice
        modelBuilder.Entity<Invoice>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.InvoiceNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.CAI).HasMaxLength(100);
            e.Property(x => x.RangeFrom).HasMaxLength(50);
            e.Property(x => x.RangeTo).HasMaxLength(50);
            e.Property(x => x.CustomerRTN).HasMaxLength(50);
            e.Property(x => x.CustomerName).HasMaxLength(200);
            e.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
            e.Property(x => x.TaxAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.ExemptAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.Total).HasColumnType("decimal(18,2)");
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasOne(x => x.Order)
                .WithOne(o => o.Invoice)
                .HasForeignKey<Invoice>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.InvoiceNumber).IsUnique();
        });

        // Reservation
        modelBuilder.Entity<Reservation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
            e.Property(x => x.CustomerPhone).HasMaxLength(50);
            e.Property(x => x.Notes).HasMaxLength(500);
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasOne(x => x.Table)
                .WithMany()
                .HasForeignKey(x => x.TableId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.TableId, x.ReservationDate });
        });

        // AuditLog
        modelBuilder.Entity<AuditLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Action).HasMaxLength(100).IsRequired();
            e.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
            e.Property(x => x.IpAddress).HasMaxLength(50);
            e.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Domain.Entities.BaseEntity baseEntity)
            {
                if (entry.State == EntityState.Added)
                    baseEntity.CreatedAt = utcNow;
                else if (entry.State == EntityState.Modified)
                    baseEntity.UpdatedAt = utcNow;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
