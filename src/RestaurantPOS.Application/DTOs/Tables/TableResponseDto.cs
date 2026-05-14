using System.Collections.Generic;

namespace RestaurantPOS.Application.DTOs.Tables;

public class TableResponseDto
{
    public int Id { get; set; }
    public int ZoneId { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }
    public string Shape { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? ActiveOrderId { get; set; }
}

public class TableZoneResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<TableResponseDto> Tables { get; set; } = new();
}
