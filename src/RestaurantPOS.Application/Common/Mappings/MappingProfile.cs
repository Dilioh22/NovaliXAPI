using AutoMapper;
using RestaurantPOS.Application.DTOs.Auth;
using RestaurantPOS.Application.DTOs.CashRegister;
using RestaurantPOS.Application.DTOs.Invoices;
using RestaurantPOS.Application.DTOs.Orders;
using RestaurantPOS.Application.DTOs.Payments;
using RestaurantPOS.Application.DTOs.Products;
using RestaurantPOS.Application.DTOs.Reports;
using RestaurantPOS.Application.DTOs.Reservations;
using RestaurantPOS.Application.DTOs.Tables;
using RestaurantPOS.Domain.Entities;

namespace RestaurantPOS.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User / Auth
        CreateMap<User, UserResponseDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName));
        CreateMap<CreateUserDto, User>()
            .ForMember(d => d.PasswordHash, o => o.Ignore());

        // Category
        CreateMap<Category, CategoryResponseDto>()
            .ForMember(d => d.ProductCount, o => o.MapFrom(s => s.Products.Count(p => p.IsActive)));
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        // Product / Modifiers
        CreateMap<Product, ProductResponseDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty));
        CreateMap<ProductModifier, ProductModifierDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        CreateMap<CreateProductModifierDto, ProductModifier>();

        // Table Zone
        CreateMap<TableZone, TableZoneResponseDto>();
        CreateMap<CreateTableZoneDto, TableZone>();

        // Table
        CreateMap<Table, TableResponseDto>()
            .ForMember(d => d.ZoneName, o => o.MapFrom(s => s.Zone != null ? s.Zone.Name : string.Empty))
            .ForMember(d => d.ActiveOrderId, o => o.MapFrom(s =>
                s.Orders.Where(ord => ord.Status != "Pagado" && ord.Status != "Cancelado" && ord.IsActive)
                        .Select(ord => (int?)ord.Id)
                        .FirstOrDefault()));
        CreateMap<CreateTableDto, Table>();
        CreateMap<UpdateTableDto, Table>();

        // Order
        CreateMap<Order, OrderResponseDto>()
            .ForMember(d => d.TableNumber, o => o.MapFrom(s => s.Table != null ? (int?)s.Table.Number : null))
            .ForMember(d => d.ZoneName, o => o.MapFrom(s => s.Table != null && s.Table.Zone != null ? s.Table.Zone.Name : null))
            .ForMember(d => d.WaiterName, o => o.MapFrom(s => s.Waiter != null ? s.Waiter.FullName : string.Empty));
        CreateMap<CreateOrderDto, Order>();

        // OrderItem
        CreateMap<OrderItem, OrderItemResponseDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty));
        CreateMap<AddOrderItemDto, OrderItem>();

        // OrderItemModifier
        CreateMap<OrderItemModifier, OrderItemModifierDto>();

        // Payment
        CreateMap<Payment, PaymentResponseDto>()
            .ForMember(d => d.OrderNumber, o => o.MapFrom(s => s.Order != null ? s.Order.OrderNumber : string.Empty))
            .ForMember(d => d.PaymentMethodName, o => o.MapFrom(s => s.PaymentMethod != null ? s.PaymentMethod.Name : string.Empty))
            .ForMember(d => d.ProcessedByName, o => o.MapFrom(s => s.ProcessedBy != null ? s.ProcessedBy.FullName : string.Empty));

        // CashRegister — use full name to avoid ambiguity with DTO namespace
        CreateMap<RestaurantPOS.Domain.Entities.CashRegister, CashRegisterResponseDto>();
        CreateMap<CashRegisterSession, CashSessionResponseDto>()
            .ForMember(d => d.CashRegisterName, o => o.MapFrom(s => s.CashRegister != null ? s.CashRegister.Name : string.Empty))
            .ForMember(d => d.OpenedByName, o => o.MapFrom(s => s.OpenedBy != null ? s.OpenedBy.FullName : string.Empty))
            .ForMember(d => d.ClosedByName, o => o.MapFrom(s => s.ClosedBy != null ? s.ClosedBy.FullName : null));

        // Reservation
        CreateMap<Reservation, ReservationResponseDto>()
            .ForMember(d => d.TableNumber, o => o.MapFrom(s => s.Table != null ? s.Table.Number : 0))
            .ForMember(d => d.ZoneName,    o => o.MapFrom(s => s.Table != null && s.Table.Zone != null ? s.Table.Zone.Name : string.Empty));
        CreateMap<CreateReservationDto, Reservation>();
        CreateMap<UpdateReservationDto, Reservation>();

        // Invoice
        CreateMap<Invoice, InvoiceResponseDto>()
            .ForMember(d => d.OrderNumber, o => o.MapFrom(s => s.Order != null ? s.Order.OrderNumber : string.Empty));
    }
}
