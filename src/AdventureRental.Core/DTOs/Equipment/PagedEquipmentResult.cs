namespace AdventureRental.Core.DTOs.Equipment;

public class PagedEquipmentResult
{
    public IEnumerable<EquipmentDto> Items { get; set; } = new List<EquipmentDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
