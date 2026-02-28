using System.ComponentModel.DataAnnotations;

namespace AdventureRental.Core.DTOs.Reservation;

public class CreateReservationRequest
{
    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string? Notes { get; set; }

    [Required, MinLength(1)]
    public IList<ReservationItemRequest> Items { get; set; } = new List<ReservationItemRequest>();
}

public class ReservationItemRequest
{
    [Required]
    public int EquipmentId { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
