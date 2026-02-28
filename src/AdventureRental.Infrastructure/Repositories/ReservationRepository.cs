using AdventureRental.Core.Entities;
using AdventureRental.Core.Interfaces;
using AdventureRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdventureRental.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _db;

    public ReservationRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Reservation>> GetAllAsync() =>
        await _db.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Items).ThenInclude(i => i.Equipment)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId) =>
        await _db.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Items).ThenInclude(i => i.Equipment)
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<Reservation?> GetByIdAsync(int id) =>
        await _db.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Items).ThenInclude(i => i.Equipment)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task AddAsync(Reservation reservation) =>
        await _db.Reservations.AddAsync(reservation);

    public void Update(Reservation reservation) =>
        _db.Reservations.Update(reservation);
}
