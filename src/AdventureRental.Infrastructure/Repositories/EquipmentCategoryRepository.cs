using AdventureRental.Core.Entities;
using AdventureRental.Core.Interfaces;
using AdventureRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdventureRental.Infrastructure.Repositories;

public class EquipmentCategoryRepository : IEquipmentCategoryRepository
{
    private readonly AppDbContext _db;

    public EquipmentCategoryRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<EquipmentCategory>> GetAllAsync() =>
        await _db.EquipmentCategories.OrderBy(c => c.Name).ToListAsync();

    public async Task<EquipmentCategory?> GetByIdAsync(int id) =>
        await _db.EquipmentCategories.FindAsync(id);

    public async Task AddAsync(EquipmentCategory category) =>
        await _db.EquipmentCategories.AddAsync(category);

    public void Update(EquipmentCategory category) =>
        _db.EquipmentCategories.Update(category);

    public void Delete(EquipmentCategory category) =>
        _db.EquipmentCategories.Remove(category);
}
