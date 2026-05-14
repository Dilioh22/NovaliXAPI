namespace RestaurantPOS.Domain.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }
    public int PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public decimal? ReceivedAmount { get; set; }
    public decimal? ChangeAmount { get; set; }
    public string? Reference { get; set; }
    public int? CashSessionId { get; set; }
    public int ProcessedById { get; set; }
    public Order Order { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public User ProcessedBy { get; set; } = null!;
    public CashRegisterSession? CashSession { get; set; }
}
