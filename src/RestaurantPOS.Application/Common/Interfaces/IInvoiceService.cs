using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantPOS.Application.DTOs.Invoices;

namespace RestaurantPOS.Application.Common.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceResponseDto> GenerateInvoiceAsync(GenerateInvoiceDto dto);
    Task<InvoiceResponseDto> GetInvoiceByOrderAsync(int orderId);
    Task<InvoiceResponseDto> GetInvoiceByIdAsync(int invoiceId);
    Task<List<InvoiceResponseDto>> GetInvoicesAsync(DateTime from, DateTime to);
    Task VoidInvoiceAsync(int invoiceId, string reason);
}
