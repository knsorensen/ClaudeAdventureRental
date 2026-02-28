using AdventureRental.Core.Entities;

namespace AdventureRental.Core.Interfaces;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetAllAsync();
    Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId);
    Task<Reservation?> GetByIdAsync(int id);
    Task AddAsync(Reservation reservation);
    void Update(Reservation reservation);
}
