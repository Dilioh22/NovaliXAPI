using System.Collections.Generic;

namespace RestaurantPOS.Domain.Entities;

public class Table : BaseEntity
{
    public int ZoneId { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; } = 4;
    public string Status { get; set; } = "Libre";
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }
    public string Shape { get; set; } = "rectangle";
    public TableZone Zone { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
