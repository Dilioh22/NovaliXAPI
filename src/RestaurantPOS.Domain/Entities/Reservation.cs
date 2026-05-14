namespace RestaurantPOS.Domain.Entities;

public class Reservation : BaseEntity
{
    public int TableId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public int PartySize { get; set; } = 1;
    public DateTime ReservationDate { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Pendiente";

    public Table Table { get; set; } = null!;
}
