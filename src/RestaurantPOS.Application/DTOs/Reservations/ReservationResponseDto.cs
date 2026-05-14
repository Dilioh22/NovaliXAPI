using System;

namespace RestaurantPOS.Application.DTOs.Reservations;

public class ReservationResponseDto
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public int TableNumber { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public int PartySize { get; set; }
    public DateTime ReservationDate { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
