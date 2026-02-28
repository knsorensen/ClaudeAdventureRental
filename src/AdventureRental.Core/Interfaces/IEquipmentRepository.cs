using AdventureRental.Core.Entities;

namespace AdventureRental.Core.Interfaces;

public interface IEquipmentRepository
{
    Task<IEnumerable<Equipment>> GetAllAsync(int page, int pageSize, int? categoryId, string? search);
    Task<int> CountAsync(int? categoryId, string? search);
    Task<Equipment?> GetByIdAsync(int id);
    Task<int> GetAvailableUnitsAsync(int equipmentId, DateTime startDate, DateTime endDate);
    Task AddAsync(Equipment equipment);
    void Update(Equipment equipment);
    void Delete(Equipment equipment);
}
