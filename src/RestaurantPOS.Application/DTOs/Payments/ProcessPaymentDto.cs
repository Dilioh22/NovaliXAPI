namespace RestaurantPOS.Application.DTOs.Payments;

public class ProcessPaymentDto
{
    public int OrderId { get; set; }
    public string PaymentMethodCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? ReceivedAmount { get; set; }
    public string? Reference { get; set; }
    public int? CashSessionId { get; set; }
    public decimal? DiscountPercent { get; set; }
}
