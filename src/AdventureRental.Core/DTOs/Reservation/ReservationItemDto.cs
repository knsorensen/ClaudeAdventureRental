namespace AdventureRental.Core.DTOs.Reservation;

public class ReservationItemDto
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal DailyRate { get; set; }
    public decimal LineTotal { get; set; }
}
