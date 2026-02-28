using AdventureRental.Core.Entities;
using AdventureRental.Core.Enums;
using AdventureRental.Core.Interfaces;
using AdventureRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdventureRental.Infrastructure.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly AppDbContext _db;

    public EquipmentRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Equipment>> GetAllAsync(int page, int pageSize, int? categoryId, string? search)
    {
        var query = _db.Equipment
            .Include(e => e.Category)
            .Where(e => e.Status != EquipmentStatus.Retired)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(e => e.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => e.Name.Contains(search) || (e.Description != null && e.Description.Contains(search)));

        return await query
            .OrderBy(e => e.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(int? categoryId, string? search)
    {
        var query = _db.Equipment
            .Where(e => e.Status != EquipmentStatus.Retired)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(e => e.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => e.Name.Contains(search) || (e.Description != null && e.Description.Contains(search)));

        return await query.CountAsync();
    }

    public async Task<Equipment?> GetByIdAsync(int id) =>
        await _db.Equipment.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<int> GetAvailableUnitsAsync(int equipmentId, DateTime startDate, DateTime endDate)
    {
        startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

        var equipment = await _db.Equipment.FindAsync(equipmentId);
        if (equipment == null) return 0;

        var bookedQuantity = await _db.ReservationItems
            .Where(ri => ri.EquipmentId == equipmentId
                && ri.Reservation.Status != ReservationStatus.Cancelled
                && ri.Reservation.StartDate < endDate
                && ri.Reservation.EndDate > startDate)
            .SumAsync(ri => (int?)ri.Quantity) ?? 0;

        return Math.Max(0, equipment.TotalUnits - bookedQuantity);
    }

    public async Task AddAsync(Equipment equipment) => await _db.Equipment.AddAsync(equipment);

    public void Update(Equipment equipment) => _db.Equipment.Update(equipment);

    public void Delete(Equipment equipment)
    {
        equipment.Status = EquipmentStatus.Retired;
        _db.Equipment.Update(equipment);
    }
}
