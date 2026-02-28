namespace AdventureRental.Core.DTOs.Customer;

public class CustomerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
