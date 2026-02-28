using AdventureRental.Core.Entities;
using AdventureRental.Core.Interfaces;
using AdventureRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdventureRental.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Customer>> GetAllAsync() =>
        await _db.Customers.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync();

    public async Task<Customer?> GetByIdAsync(int id) =>
        await _db.Customers.FindAsync(id);

    public async Task<Customer?> GetByUserIdAsync(string userId) =>
        await _db.Customers.FirstOrDefaultAsync(c => c.UserId == userId);

    public async Task<Customer?> GetByEmailAsync(string email) =>
        await _db.Customers.FirstOrDefaultAsync(c => c.Email == email);

    public async Task AddAsync(Customer customer) =>
        await _db.Customers.AddAsync(customer);

    public void Update(Customer customer) =>
        _db.Customers.Update(customer);
}
