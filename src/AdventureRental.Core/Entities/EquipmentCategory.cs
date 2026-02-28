namespace AdventureRental.Core.Entities;

public class EquipmentCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }

    public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
