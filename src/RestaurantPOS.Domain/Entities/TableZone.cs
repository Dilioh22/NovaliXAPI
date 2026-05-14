using System.Collections.Generic;

namespace RestaurantPOS.Domain.Entities;

public class TableZone : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<Table> Tables { get; set; } = new List<Table>();
}
