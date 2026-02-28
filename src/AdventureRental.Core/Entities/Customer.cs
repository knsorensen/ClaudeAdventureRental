namespace AdventureRental.Core.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
