using System;

namespace RestaurantPOS.Application.DTOs.Invoices;

public class GenerateInvoiceDto
{
    public int OrderId { get; set; }
    public string? CustomerRTN { get; set; }
    public string? CustomerName { get; set; }
    public decimal ExemptAmount { get; set; }
}

public class InvoiceResponseDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? CAI { get; set; }
    public string? RangeFrom { get; set; }
    public string? RangeTo { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? CustomerRTN { get; set; }
    public string? CustomerName { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ExemptAmount { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
