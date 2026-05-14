namespace RestaurantPOS.Application.DTOs.CashRegister;

public class CloseSessionDto
{
    public decimal ClosingAmount { get; set; }
    public string? Notes { get; set; }
}
