using System.ComponentModel.DataAnnotations;

namespace AdventureRental.Core.DTOs.Category;

public class CreateCategoryRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? IconUrl { get; set; }
}
