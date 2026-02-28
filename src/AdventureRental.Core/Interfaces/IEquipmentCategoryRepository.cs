using AdventureRental.Core.Entities;

namespace AdventureRental.Core.Interfaces;

public interface IEquipmentCategoryRepository
{
    Task<IEnumerable<EquipmentCategory>> GetAllAsync();
    Task<EquipmentCategory?> GetByIdAsync(int id);
    Task AddAsync(EquipmentCategory category);
    void Update(EquipmentCategory category);
    void Delete(EquipmentCategory category);
}
