using System.ComponentModel.DataAnnotations;
using AdventureRental.Core.Enums;

namespace AdventureRental.Core.DTOs.Equipment;

public class CreateEquipmentRequest
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    [Required, Range(0.01, double.MaxValue)]
    public decimal DailyRate { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int TotalUnits { get; set; }

    public EquipmentStatus Status { get; set; } = EquipmentStatus.Active;
}
