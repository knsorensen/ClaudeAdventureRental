using AdventureRental.Core.Entities;

namespace AdventureRental.Core.Interfaces;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByUserIdAsync(string userId);
    Task<Customer?> GetByEmailAsync(string email);
    Task AddAsync(Customer customer);
    void Update(Customer customer);
}
