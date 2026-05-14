using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Application.Common.Exceptions;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Invoices;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Interfaces;

namespace RestaurantPOS.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IAppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InvoiceService(IAppDbContext context, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<InvoiceResponseDto> GenerateInvoiceAsync(GenerateInvoiceDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.Invoice)
            .FirstOrDefaultAsync(o => o.Id == dto.OrderId)
            ?? throw new NotFoundException(nameof(Order), dto.OrderId);

        if (order.Invoice != null && order.Invoice.Status == "Emitida")
            throw new BusinessRuleException("Esta orden ya tiene una factura emitida.");

        if (order.Status != "Pagado")
            throw new BusinessRuleException("Solo se pueden facturar órdenes pagadas.");

        var invoiceNumber = await GenerateInvoiceNumberAsync();

        var invoice = new Invoice
        {
            OrderId = dto.OrderId,
            InvoiceNumber = invoiceNumber,
            CustomerRTN = dto.CustomerRTN,
            CustomerName = dto.CustomerName ?? order.CustomerName,
            Subtotal = order.Subtotal - order.DiscountAmount,
            TaxAmount = order.TaxAmount,
            ExemptAmount = dto.ExemptAmount,
            Total = order.Total,
            Status = "Emitida"
        };

        await _context.Invoices.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        return await GetInvoiceByIdAsync(invoice.Id);
    }

    public async Task<InvoiceResponseDto> GetInvoiceByOrderAsync(int orderId)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Order)
            .FirstOrDefaultAsync(i => i.OrderId == orderId)
            ?? throw new NotFoundException("Invoice for order", orderId);
        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<InvoiceResponseDto> GetInvoiceByIdAsync(int invoiceId)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Order)
            .FirstOrDefaultAsync(i => i.Id == invoiceId)
            ?? throw new NotFoundException(nameof(Invoice), invoiceId);
        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<List<InvoiceResponseDto>> GetInvoicesAsync(DateTime from, DateTime to)
    {
        var toEnd = to.Date.AddDays(1).AddTicks(-1);
        var invoices = await _context.Invoices
            .Include(i => i.Order)
            .Where(i => i.CreatedAt >= from && i.CreatedAt <= toEnd && i.Status == "Emitida")
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
        return _mapper.Map<List<InvoiceResponseDto>>(invoices);
    }

    public async Task VoidInvoiceAsync(int invoiceId, string reason)
    {
        var invoice = await _context.Invoices.FindAsync(invoiceId)
            ?? throw new NotFoundException(nameof(Invoice), invoiceId);

        if (invoice.Status == "Anulada")
            throw new BusinessRuleException("La factura ya fue anulada.");

        invoice.Status = "Anulada";
        invoice.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<string> GenerateInvoiceNumberAsync()
    {
        var count = await _context.Invoices.CountAsync();
        return $"FAC-{DateTime.UtcNow:yyyy}-{(count + 1):D6}";
    }
}
