using AdventureRental.Core.Enums;

namespace AdventureRental.Core.Entities;

public class Equipment
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal DailyRate { get; set; }
    public int TotalUnits { get; set; }
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Active;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public EquipmentCategory Category { get; set; } = null!;
    public ICollection<ReservationItem> ReservationItems { get; set; } = new List<ReservationItem>();
}
