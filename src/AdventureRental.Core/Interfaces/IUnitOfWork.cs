namespace AdventureRental.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEquipmentRepository Equipment { get; }
    IEquipmentCategoryRepository EquipmentCategories { get; }
    IReservationRepository Reservations { get; }
    ICustomerRepository Customers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
