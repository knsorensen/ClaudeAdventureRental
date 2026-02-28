using AdventureRental.Core.Interfaces;
using AdventureRental.Infrastructure.Repositories;

namespace AdventureRental.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public IEquipmentRepository Equipment { get; }
    public IEquipmentCategoryRepository EquipmentCategories { get; }
    public IReservationRepository Reservations { get; }
    public ICustomerRepository Customers { get; }

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Equipment = new EquipmentRepository(db);
        EquipmentCategories = new EquipmentCategoryRepository(db);
        Reservations = new ReservationRepository(db);
        Customers = new CustomerRepository(db);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);

    public void Dispose() => _db.Dispose();
}
