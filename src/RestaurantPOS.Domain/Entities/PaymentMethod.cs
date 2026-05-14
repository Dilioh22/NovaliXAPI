namespace RestaurantPOS.Domain.Entities;

public class PaymentMethod : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
