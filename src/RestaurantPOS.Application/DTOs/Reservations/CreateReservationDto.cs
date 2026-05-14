using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantPOS.Application.DTOs.Reservations;

public class CreateReservationDto
{
    [Required]
    public int TableId { get; set; }

    [Required, MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? CustomerPhone { get; set; }

    [Range(1, 50)]
    public int PartySize { get; set; } = 1;

    [Required]
    public DateTime ReservationDate { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
