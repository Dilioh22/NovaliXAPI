namespace RestaurantPOS.Application.DTOs.Payments;

public class PaymentResponseDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int PaymentMethodId { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? ReceivedAmount { get; set; }
    public decimal? ChangeAmount { get; set; }
    public string? Reference { get; set; }
    public string ProcessedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsOrderFullyPaid { get; set; }
}
