using AdventureRental.Core.Enums;

namespace AdventureRental.Core.DTOs.Equipment;

public class EquipmentDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal DailyRate { get; set; }
    public int TotalUnits { get; set; }
    public EquipmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
