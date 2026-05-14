namespace RestaurantPOS.Application.DTOs.CashRegister;

public class OpenSessionDto
{
    public int CashRegisterId { get; set; }
    public decimal OpeningAmount { get; set; }
    public string? Notes { get; set; }
}
