namespace AdventureRental.Core.DTOs.Category;

public class EquipmentCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
}
