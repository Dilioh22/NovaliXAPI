namespace RestaurantPOS.Application.DTOs.Tables;

public class CreateTableDto
{
    public int ZoneId { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; } = 4;
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }
    public string Shape { get; set; } = "rectangle";
}

public class UpdateTableDto
{
    public int ZoneId { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; } = 4;
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }
    public string Shape { get; set; } = "rectangle";
}

public class CreateTableZoneDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
