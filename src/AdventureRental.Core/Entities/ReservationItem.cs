namespace AdventureRental.Core.Entities;

public class ReservationItem
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public int EquipmentId { get; set; }
    public int Quantity { get; set; }
    public decimal DailyRate { get; set; }
    public decimal LineTotal { get; set; }

    public Reservation Reservation { get; set; } = null!;
    public Equipment Equipment { get; set; } = null!;
}
