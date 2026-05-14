using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Infrastructure.Data;

namespace RestaurantPOS.Infrastructure.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        await SeedPaymentMethodsAsync(context);
        await SeedCategoriesAndProductsAsync(context);
        await SeedTableZonesAndTablesAsync(context);
        await SeedUsersAsync(context);
        await SeedCashRegistersAsync(context);
    }

    private static async Task SeedPaymentMethodsAsync(AppDbContext context)
    {
        if (await context.PaymentMethods.AnyAsync()) return;

        context.PaymentMethods.AddRange(
            new PaymentMethod { Name = "Efectivo", Code = "CASH" },
            new PaymentMethod { Name = "Tarjeta de Crédito/Débito", Code = "CARD" },
            new PaymentMethod { Name = "Transferencia", Code = "TRANSFER" }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAndProductsAsync(AppDbContext context)
    {
        if (await context.Categories.AnyAsync()) return;

        var entradas = new Category { Name = "Entradas", DisplayOrder = 1, IconName = "appetizer" };
        var bebidas = new Category { Name = "Bebidas", DisplayOrder = 2, IconName = "drink" };
        var platosPrincipales = new Category { Name = "Platos Principales", DisplayOrder = 3, IconName = "main_course" };
        var postres = new Category { Name = "Postres", DisplayOrder = 4, IconName = "dessert" };

        context.Categories.AddRange(entradas, bebidas, platosPrincipales, postres);
        await context.SaveChangesAsync();

        // Entradas
        context.Products.AddRange(
            new Product
            {
                CategoryId = entradas.Id, Name = "Sopa del Día", Price = 85.00m,
                Description = "Sopa casera del día", PreparationTime = 10, DisplayOrder = 1
            },
            new Product
            {
                CategoryId = entradas.Id, Name = "Ensalada César", Price = 120.00m,
                Description = "Lechuga romana, crutones, parmesano y aderezo César", PreparationTime = 8, DisplayOrder = 2
            },
            new Product
            {
                CategoryId = entradas.Id, Name = "Nachos con Queso", Price = 130.00m,
                Description = "Tortillas con queso fundido, jalapeños y guacamole", PreparationTime = 12, DisplayOrder = 3
            }
        );

        // Bebidas
        context.Products.AddRange(
            new Product
            {
                CategoryId = bebidas.Id, Name = "Agua Natural", Price = 30.00m,
                Description = "Agua embotellada 600ml", PreparationTime = 1, DisplayOrder = 1
            },
            new Product
            {
                CategoryId = bebidas.Id, Name = "Refresco", Price = 45.00m,
                Description = "Coca-Cola, Pepsi, Sprite 355ml", PreparationTime = 1, DisplayOrder = 2
            },
            new Product
            {
                CategoryId = bebidas.Id, Name = "Jugo Natural", Price = 65.00m,
                Description = "Naranja, mango o piña 350ml", PreparationTime = 5, DisplayOrder = 3
            },
            new Product
            {
                CategoryId = bebidas.Id, Name = "Café Americano", Price = 55.00m,
                Description = "Café de grano tostado", PreparationTime = 3, DisplayOrder = 4
            }
        );

        // Platos Principales
        context.Products.AddRange(
            new Product
            {
                CategoryId = platosPrincipales.Id, Name = "Pollo a la Plancha", Price = 220.00m,
                Description = "Pechuga de pollo a la plancha con arroz y ensalada", PreparationTime = 20, DisplayOrder = 1
            },
            new Product
            {
                CategoryId = platosPrincipales.Id, Name = "Chuleta de Cerdo", Price = 250.00m,
                Description = "Chuleta de cerdo asada con papas fritas", PreparationTime = 25, DisplayOrder = 2
            },
            new Product
            {
                CategoryId = platosPrincipales.Id, Name = "Bistec a la Hondureña", Price = 280.00m,
                Description = "Bistec de res con chimol, arroz, frijoles y tortillas", PreparationTime = 25, DisplayOrder = 3
            },
            new Product
            {
                CategoryId = platosPrincipales.Id, Name = "Pasta Carbonara", Price = 190.00m,
                Description = "Pasta con salsa carbonara y tocino", PreparationTime = 18, DisplayOrder = 4
            },
            new Product
            {
                CategoryId = platosPrincipales.Id, Name = "Hamburguesa Clásica", Price = 175.00m,
                Description = "Hamburguesa de res con papas fritas", PreparationTime = 15, DisplayOrder = 5,
                Modifiers = new List<ProductModifier>
                {
                    new() { Name = "Queso Extra", PriceAdjustment = 20.00m },
                    new() { Name = "Tocino", PriceAdjustment = 25.00m },
                    new() { Name = "Sin Cebolla", PriceAdjustment = 0m }
                }
            }
        );

        // Postres
        context.Products.AddRange(
            new Product
            {
                CategoryId = postres.Id, Name = "Flan de Caramelo", Price = 85.00m,
                Description = "Flan casero con salsa de caramelo", PreparationTime = 5, DisplayOrder = 1
            },
            new Product
            {
                CategoryId = postres.Id, Name = "Pastel de Chocolate", Price = 95.00m,
                Description = "Pastel húmedo de chocolate con helado", PreparationTime = 5, DisplayOrder = 2
            }
        );

        await context.SaveChangesAsync();
    }

    private static async Task SeedTableZonesAndTablesAsync(AppDbContext context)
    {
        if (await context.TableZones.AnyAsync()) return;

        var interior = new TableZone { Name = "Interior", Description = "Salón interior climatizado" };
        var exterior = new TableZone { Name = "Terraza", Description = "Área exterior al aire libre" };
        var bar = new TableZone { Name = "Bar", Description = "Área de bar" };

        context.TableZones.AddRange(interior, exterior, bar);
        await context.SaveChangesAsync();

        // Interior tables 1-8
        for (int i = 1; i <= 8; i++)
        {
            await context.Tables.AddAsync(new Table
            {
                ZoneId = interior.Id,
                Number = i,
                Capacity = i <= 4 ? 4 : 6,
                PositionX = ((i - 1) % 4) * 150,
                PositionY = ((i - 1) / 4) * 150
            });
        }

        // Terraza tables 9-14
        for (int i = 9; i <= 14; i++)
        {
            await context.Tables.AddAsync(new Table
            {
                ZoneId = exterior.Id,
                Number = i,
                Capacity = 4,
                PositionX = ((i - 9) % 3) * 150,
                PositionY = ((i - 9) / 3) * 150
            });
        }

        // Bar tables 15-16
        context.Tables.AddRange(
            new Table { ZoneId = bar.Id, Number = 15, Capacity = 2, Shape = "circle" },
            new Table { ZoneId = bar.Id, Number = 16, Capacity = 2, Shape = "circle" }
        );

        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        context.Users.AddRange(
            new User
            {
                Email = "admin@restaurante.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "Administrador",
                LastName = "Sistema",
                Role = "Admin",
                Pin = "1234"
            },
            new User
            {
                Email = "cajero@restaurante.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cajero123!"),
                FirstName = "Carlos",
                LastName = "García",
                Role = "Cajero",
                Pin = "2222"
            },
            new User
            {
                Email = "mesero@restaurante.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Mesero123!"),
                FirstName = "María",
                LastName = "López",
                Role = "Mesero",
                Pin = "3333"
            },
            new User
            {
                Email = "cocinero@restaurante.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cocina123!"),
                FirstName = "Juan",
                LastName = "Martínez",
                Role = "Cocinero",
                Pin = "4444"
            }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedCashRegistersAsync(AppDbContext context)
    {
        if (await context.CashRegisters.AnyAsync()) return;

        context.CashRegisters.AddRange(
            new CashRegister { Name = "Caja Principal", Location = "Recepción" },
            new CashRegister { Name = "Caja Bar", Location = "Bar" }
        );
        await context.SaveChangesAsync();
    }
}
