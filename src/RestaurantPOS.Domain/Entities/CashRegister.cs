using System.Collections.Generic;

namespace RestaurantPOS.Domain.Entities;

public class CashRegister : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public ICollection<CashRegisterSession> Sessions { get; set; } = new List<CashRegisterSession>();
}
