using System.ComponentModel.DataAnnotations;

namespace AdventureRental.Core.DTOs.Customer;

public class UpdateCustomerRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    public string? Phone { get; set; }
    public string? Address { get; set; }
}
