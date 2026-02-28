using AdventureRental.Core.Enums;

namespace AdventureRental.Core.DTOs.Reservation;

public class ReservationDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ReservationStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IEnumerable<ReservationItemDto> Items { get; set; } = new List<ReservationItemDto>();
}
