using System.ComponentModel.DataAnnotations;

namespace AdventureRental.Core.DTOs.Customer;

public class CreateCustomerRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? UserId { get; set; }
}
