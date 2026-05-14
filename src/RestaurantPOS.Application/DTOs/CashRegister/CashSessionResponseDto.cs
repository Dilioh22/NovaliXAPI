using System;

namespace RestaurantPOS.Application.DTOs.CashRegister;

public class CashSessionResponseDto
{
    public int Id { get; set; }
    public int CashRegisterId { get; set; }
    public string CashRegisterName { get; set; } = string.Empty;
    public int OpenedById { get; set; }
    public string OpenedByName { get; set; } = string.Empty;
    public int? ClosedById { get; set; }
    public string? ClosedByName { get; set; }
    public decimal OpeningAmount { get; set; }
    public decimal? ClosingAmount { get; set; }
    public decimal? ExpectedAmount { get; set; }
    public decimal? Difference { get; set; }
    public decimal? TotalCash { get; set; }
    public decimal? TotalCard { get; set; }
    public decimal? TotalTransfer { get; set; }
    public decimal? TotalSales { get; set; }
    public int? TransactionCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? Notes { get; set; }
}

public class CashRegisterResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public bool IsActive { get; set; }
    public CashSessionResponseDto? CurrentSession { get; set; }
}
