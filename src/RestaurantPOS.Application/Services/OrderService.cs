using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Application.Common.Exceptions;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Orders;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Interfaces;

namespace RestaurantPOS.Application.Services;

public class OrderService : IOrderService
{
    private readonly IAppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IOrderNumberService _orderNumberService;

    public OrderService(IAppDbContext context, IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUser, IOrderNumberService orderNumberService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
        _orderNumberService = orderNumberService;
    }

    private IQueryable<Order> OrdersWithIncludes() =>
        _context.Orders
            .Include(o => o.Table).ThenInclude(t => t != null ? t.Zone : null)
            .Include(o => o.Waiter)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Items).ThenInclude(i => i.Modifiers);

    public async Task<List<OrderResponseDto>> GetAllOrdersAsync(string? status = null)
    {
        var query = OrdersWithIncludes().Where(o => o.IsActive);
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(o => o.Status == status);

        var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
        return _mapper.Map<List<OrderResponseDto>>(orders);
    }

    public async Task<List<OrderResponseDto>> GetActiveOrdersAsync()
    {
        var orders = await OrdersWithIncludes()
            .Where(o => o.IsActive && o.Status != "Pagado" && o.Status != "Cancelado")
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
        return _mapper.Map<List<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto> GetOrderByIdAsync(int id)
    {
        var order = await OrdersWithIncludes().FirstOrDefaultAsync(o => o.Id == id && o.IsActive)
            ?? throw new NotFoundException(nameof(Order), id);
        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
    {
        if (dto.TableId.HasValue)
        {
            var table = await _context.Tables.FindAsync(dto.TableId.Value)
                ?? throw new NotFoundException(nameof(Table), dto.TableId.Value);

            if (table.Status == "Ocupada")
            {
                var activeOrder = await _context.Orders
                    .AnyAsync(o => o.TableId == dto.TableId && o.IsActive
                        && o.Status != "Pagado" && o.Status != "Cancelado");
                if (activeOrder)
                    throw new BusinessRuleException("La mesa ya tiene una orden activa.");
            }

            table.Status = "Ocupada";
            table.UpdatedAt = DateTime.UtcNow;
        }

        var orderNumber = await _orderNumberService.GenerateAsync();
        var waiterId = _currentUser.UserId
            ?? throw new BusinessRuleException("Usuario no autenticado.");

        var order = new Order
        {
            OrderNumber = orderNumber,
            TableId = dto.TableId,
            WaiterId = waiterId,
            OrderType = dto.OrderType,
            Notes = dto.Notes,
            CustomerName = dto.CustomerName,
            CustomerPhone = dto.CustomerPhone,
            Status = "Pendiente",
            TaxRate = 0.15m
        };

        foreach (var itemDto in dto.Items)
        {
            var product = await _context.Products.Include(p => p.Modifiers)
                .FirstOrDefaultAsync(p => p.Id == itemDto.ProductId)
                ?? throw new NotFoundException(nameof(Product), itemDto.ProductId);

            var item = new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price,
                Notes = itemDto.Notes,
                Status = "Pendiente"
            };

            foreach (var modId in itemDto.ModifierIds)
            {
                var mod = product.Modifiers.FirstOrDefault(m => m.Id == modId)
                    ?? throw new NotFoundException(nameof(ProductModifier), modId);
                item.Modifiers.Add(new OrderItemModifier
                {
                    ProductModifierId = mod.Id,
                    Name = mod.Name,
                    PriceAdjustment = mod.PriceAdjustment
                });
            }

            item.Subtotal = (item.UnitPrice + item.Modifiers.Sum(m => m.PriceAdjustment)) * item.Quantity;
            order.Items.Add(item);
        }

        RecalculateTotals(order);

        await _context.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        return await GetOrderByIdAsync(order.Id);
    }

    public async Task<OrderResponseDto> AddItemToOrderAsync(int orderId, AddOrderItemDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Modifiers)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException(nameof(Order), orderId);

        if (order.Status is "Pagado" or "Cancelado")
            throw new BusinessRuleException("No se puede agregar items a una orden pagada o cancelada.");

        var product = await _context.Products.Include(p => p.Modifiers)
            .FirstOrDefaultAsync(p => p.Id == dto.ProductId)
            ?? throw new NotFoundException(nameof(Product), dto.ProductId);

        var item = new OrderItem
        {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UnitPrice = product.Price,
            Notes = dto.Notes,
            Status = "Pendiente"
        };

        foreach (var modId in dto.ModifierIds)
        {
            var mod = product.Modifiers.FirstOrDefault(m => m.Id == modId)
                ?? throw new NotFoundException(nameof(ProductModifier), modId);
            item.Modifiers.Add(new OrderItemModifier
            {
                ProductModifierId = mod.Id,
                Name = mod.Name,
                PriceAdjustment = mod.PriceAdjustment
            });
        }

        item.Subtotal = (item.UnitPrice + item.Modifiers.Sum(m => m.PriceAdjustment)) * item.Quantity;
        order.Items.Add(item);
        order.UpdatedAt = DateTime.UtcNow;

        RecalculateTotals(order);
        await _unitOfWork.SaveChangesAsync();

        return await GetOrderByIdAsync(orderId);
    }

    public async Task<OrderResponseDto> UpdateOrderAsync(int orderId, UpdateOrderDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException(nameof(Order), orderId);

        if (order.Status is "Pagado" or "Cancelado")
            throw new BusinessRuleException("No se puede modificar una orden pagada o cancelada.");

        order.Notes = dto.Notes;
        order.CustomerName = dto.CustomerName;
        order.CustomerPhone = dto.CustomerPhone;
        order.DiscountPercent = dto.DiscountPercent;
        order.DiscountAmount = dto.DiscountAmount;
        order.TipAmount = dto.TipAmount;
        order.UpdatedAt = DateTime.UtcNow;

        RecalculateTotals(order);
        await _unitOfWork.SaveChangesAsync();

        return await GetOrderByIdAsync(orderId);
    }

    public async Task<OrderResponseDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
    {
        var order = await _context.Orders.FindAsync(orderId)
            ?? throw new NotFoundException(nameof(Order), orderId);

        order.Status = dto.Status;
        order.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return await GetOrderByIdAsync(orderId);
    }

    public async Task RemoveOrderItemAsync(int orderId, int itemId)
    {
        var order = await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Modifiers)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException(nameof(Order), orderId);

        var item = order.Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new NotFoundException(nameof(OrderItem), itemId);

        if (item.Status == "EnPreparacion" || item.SentToKitchenAt.HasValue)
            throw new BusinessRuleException("No se puede eliminar un item que ya está en preparación.");

        order.Items.Remove(item);
        _context.OrderItems.Remove(item);
        order.UpdatedAt = DateTime.UtcNow;

        RecalculateTotals(order);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<OrderResponseDto> SendToKitchenAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException(nameof(Order), orderId);

        var pendingItems = order.Items.Where(i => i.Status == "Pendiente" && !i.SentToKitchenAt.HasValue).ToList();
        if (!pendingItems.Any())
            throw new BusinessRuleException("No hay items pendientes de enviar a cocina.");

        foreach (var item in pendingItems)
        {
            item.Status = "EnPreparacion";
            item.SentToKitchenAt = DateTime.UtcNow;
        }

        order.Status = "EnPreparacion";
        order.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return await GetOrderByIdAsync(orderId);
    }

    public async Task CancelOrderAsync(int orderId, string reason)
    {
        var order = await _context.Orders
            .Include(o => o.Table)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException(nameof(Order), orderId);

        if (order.Status == "Pagado")
            throw new BusinessRuleException("No se puede cancelar una orden ya pagada.");

        order.Status = "Cancelado";
        order.Notes = string.IsNullOrWhiteSpace(order.Notes)
            ? $"Cancelado: {reason}"
            : $"{order.Notes} | Cancelado: {reason}";
        order.UpdatedAt = DateTime.UtcNow;

        if (order.Table != null)
        {
            order.Table.Status = "Libre";
            order.Table.UpdatedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<OrderResponseDto> AddItemsToOrderAsync(int orderId, AddOrderItemsDto dto)
    {
        foreach (var item in dto.Items)
            await AddItemToOrderAsync(orderId, item);
        return await GetOrderByIdAsync(orderId);
    }

    public async Task MarkOrderItemReadyAsync(int orderId, int itemId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException(nameof(Order), orderId);

        var item = order.Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new NotFoundException(nameof(OrderItem), itemId);

        item.Status = "Listo";
        item.ReadyAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<OrderResponseDto> MarkOrderReadyAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException(nameof(Order), orderId);

        order.Status = "Listo";
        order.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return await GetOrderByIdAsync(orderId);
    }

    public async Task<List<OrderResponseDto>> GetActiveOrdersForKitchenAsync()
    {
        var orders = await OrdersWithIncludes()
            .Where(o => o.IsActive && o.Status == "EnPreparacion")
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();
        return _mapper.Map<List<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto> GetOrderByTableAsync(int tableId)
    {
        var order = await OrdersWithIncludes()
            .FirstOrDefaultAsync(o => o.TableId == tableId && o.IsActive
                && o.Status != "Pagado" && o.Status != "Cancelado")
            ?? throw new NotFoundException("Active order for table", tableId);
        return _mapper.Map<OrderResponseDto>(order);
    }

    private static void RecalculateTotals(Order order)
    {
        order.Subtotal = order.Items.Sum(i => i.Subtotal);

        if (order.DiscountPercent > 0)
            order.DiscountAmount = order.Subtotal * (order.DiscountPercent / 100);

        var discounted = order.Subtotal - order.DiscountAmount;
        order.TaxAmount = discounted * order.TaxRate;
        order.Total = discounted + order.TaxAmount + order.TipAmount;
    }

}
