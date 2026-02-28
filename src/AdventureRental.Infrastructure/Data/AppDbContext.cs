using AdventureRental.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdventureRental.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<EquipmentCategory> EquipmentCategories => Set<EquipmentCategory>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ReservationItem> ReservationItems => Set<ReservationItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Equipment equipment)
            {
                if (entry.State == EntityState.Added)
                    equipment.CreatedAt = now;
                if (entry.State is EntityState.Added or EntityState.Modified)
                    equipment.UpdatedAt = now;
            }
            else if (entry.Entity is Reservation reservation)
            {
                if (entry.State == EntityState.Added)
                    reservation.CreatedAt = now;
                if (entry.State is EntityState.Added or EntityState.Modified)
                    reservation.UpdatedAt = now;
            }
            else if (entry.Entity is Customer customer && entry.State == EntityState.Added)
            {
                customer.CreatedAt = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
