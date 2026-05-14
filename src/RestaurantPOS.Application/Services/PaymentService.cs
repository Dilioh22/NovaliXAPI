using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Application.Common.Exceptions;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Payments;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Interfaces;

namespace RestaurantPOS.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IAppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public PaymentService(IAppDbContext context, IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUser)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.Payments)
            .Include(o => o.Items)
            .Include(o => o.Table)
            .FirstOrDefaultAsync(o => o.Id == dto.OrderId)
            ?? throw new NotFoundException(nameof(Order), dto.OrderId);

        if (order.Status == "Pagado")
            throw new BusinessRuleException("La orden ya fue pagada.");

        if (order.Status == "Cancelado")
            throw new BusinessRuleException("No se puede pagar una orden cancelada.");

        var paymentMethod = await _context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.Code == dto.PaymentMethodCode && pm.IsActive)
            ?? throw new NotFoundException("PaymentMethod", dto.PaymentMethodCode);

        if (dto.DiscountPercent.HasValue && dto.DiscountPercent.Value > 0)
        {
            order.DiscountPercent = dto.DiscountPercent.Value;
            order.DiscountAmount = order.Subtotal * (dto.DiscountPercent.Value / 100);
            var discounted = order.Subtotal - order.DiscountAmount;
            order.TaxAmount = discounted * order.TaxRate;
            order.Total = discounted + order.TaxAmount + order.TipAmount;
            order.UpdatedAt = DateTime.UtcNow;
        }

        var totalPaid = order.Payments.Sum(p => p.Amount);
        var remaining = order.Total - totalPaid;

        if (dto.Amount > remaining + 0.01m)
            throw new BusinessRuleException($"El monto excede el saldo pendiente de {remaining:C}.");

        decimal? change = null;
        if (dto.ReceivedAmount.HasValue)
        {
            if (dto.ReceivedAmount < dto.Amount)
                throw new BusinessRuleException("El monto recibido es menor que el monto a pagar.");
            change = dto.ReceivedAmount - dto.Amount;
        }

        var processedById = _currentUser.UserId
            ?? throw new BusinessRuleException("Usuario no autenticado.");

        var payment = new Payment
        {
            OrderId = dto.OrderId,
            PaymentMethodId = paymentMethod.Id,
            Amount = dto.Amount,
            ReceivedAmount = dto.ReceivedAmount,
            ChangeAmount = change,
            Reference = dto.Reference,
            CashSessionId = dto.CashSessionId,
            ProcessedById = processedById
        };

        await _context.Payments.AddAsync(payment);

        var newTotalPaid = totalPaid + dto.Amount;
        if (newTotalPaid >= order.Total - 0.01m)
        {
            order.Status = "Pagado";
            order.UpdatedAt = DateTime.UtcNow;

            if (order.Table != null)
            {
                order.Table.Status = "Libre";
                order.Table.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _unitOfWork.SaveChangesAsync();

        var savedPayment = await _context.Payments
            .Include(p => p.Order)
            .Include(p => p.PaymentMethod)
            .Include(p => p.ProcessedBy)
            .FirstOrDefaultAsync(p => p.Id == payment.Id);

        var result = _mapper.Map<PaymentResponseDto>(savedPayment);
        result.IsOrderFullyPaid = newTotalPaid >= order.Total - 0.01m;
        return result;
    }

    public async Task<List<PaymentResponseDto>> GetPaymentsByOrderAsync(int orderId)
    {
        var payments = await _context.Payments
            .Include(p => p.Order)
            .Include(p => p.PaymentMethod)
            .Include(p => p.ProcessedBy)
            .Where(p => p.OrderId == orderId)
            .ToListAsync();
        return _mapper.Map<List<PaymentResponseDto>>(payments);
    }

    public async Task<List<PaymentMethodDto>> GetPaymentMethodsAsync()
    {
        var methods = await _context.PaymentMethods
            .Where(pm => pm.IsActive)
            .ToListAsync();
        return methods.Select(m => new PaymentMethodDto
        {
            Id = m.Id,
            Name = m.Name,
            Code = m.Code
        }).ToList();
    }
}
