using System;
using System.Collections.Generic;

namespace RestaurantPOS.Domain.Entities;

public class CashRegisterSession : BaseEntity
{
    public int CashRegisterId { get; set; }
    public int OpenedById { get; set; }
    public int? ClosedById { get; set; }
    public decimal OpeningAmount { get; set; }
    public decimal? ClosingAmount { get; set; }
    public decimal? ExpectedAmount { get; set; }
    public decimal? Difference { get; set; }
    public decimal? TotalCash { get; set; }
    public decimal? TotalCard { get; set; }
    public decimal? TotalTransfer { get; set; }
    public decimal? TotalSales { get; set; }
    public int? TransactionCount { get; set; }
    public string Status { get; set; } = "Open";
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    public string? Notes { get; set; }
    public CashRegister CashRegister { get; set; } = null!;
    public User OpenedBy { get; set; } = null!;
    public User? ClosedBy { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
