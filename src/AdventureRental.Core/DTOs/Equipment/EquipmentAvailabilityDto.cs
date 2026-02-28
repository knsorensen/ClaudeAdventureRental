namespace AdventureRental.Core.DTOs.Equipment;

public class EquipmentAvailabilityDto
{
    public int EquipmentId { get; set; }
    public int TotalUnits { get; set; }
    public int AvailableUnits { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
