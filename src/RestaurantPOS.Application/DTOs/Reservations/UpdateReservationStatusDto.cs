using System.ComponentModel.DataAnnotations;

namespace RestaurantPOS.Application.DTOs.Reservations;

public class UpdateReservationStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
