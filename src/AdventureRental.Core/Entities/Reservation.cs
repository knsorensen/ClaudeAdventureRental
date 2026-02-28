using AdventureRental.Core.Enums;

namespace AdventureRental.Core.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Customer Customer { get; set; } = null!;
    public ICollection<ReservationItem> Items { get; set; } = new List<ReservationItem>();
}
