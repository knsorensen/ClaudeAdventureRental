using AdventureRental.Core.Entities;
using AdventureRental.Core.Enums;
using AdventureRental.Infrastructure.Data;
using AdventureRental.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AdventureRental.Tests;

public class EquipmentRepositoryTests
{
    private static AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveEquipment()
    {
        await using var db = CreateInMemoryDb();
        db.EquipmentCategories.Add(new EquipmentCategory { Id = 1, Name = "Camping" });
        db.Equipment.AddRange(
            new Equipment { Name = "Tent", CategoryId = 1, DailyRate = 25, TotalUnits = 5, Status = EquipmentStatus.Active },
            new Equipment { Name = "Old Tent", CategoryId = 1, DailyRate = 15, TotalUnits = 2, Status = EquipmentStatus.Retired }
        );
        await db.SaveChangesAsync();

        var repo = new EquipmentRepository(db);
        var result = await repo.GetAllAsync(1, 10, null, null);

        Assert.Single(result);
        Assert.Equal("Tent", result.First().Name);
    }

    [Fact]
    public async Task GetAvailableUnitsAsync_ReturnsCorrectCount()
    {
        await using var db = CreateInMemoryDb();
        var category = new EquipmentCategory { Name = "Water" };
        db.EquipmentCategories.Add(category);
        await db.SaveChangesAsync();

        var equipment = new Equipment
        {
            Name = "Kayak",
            CategoryId = category.Id,
            DailyRate = 50,
            TotalUnits = 10,
            Status = EquipmentStatus.Active
        };
        db.Equipment.Add(equipment);
        await db.SaveChangesAsync();

        // Add a reservation for 3 units
        var customer = new Customer { FirstName = "Test", LastName = "User", Email = "test@test.com" };
        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        var reservation = new Reservation
        {
            CustomerId = customer.Id,
            StartDate = new DateTime(2026, 6, 1),
            EndDate = new DateTime(2026, 6, 5),
            Status = ReservationStatus.Confirmed,
            Items = new List<ReservationItem>
            {
                new() { EquipmentId = equipment.Id, Quantity = 3, DailyRate = 50, LineTotal = 600 }
            }
        };
        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();

        var repo = new EquipmentRepository(db);
        var available = await repo.GetAvailableUnitsAsync(
            equipment.Id,
            new DateTime(2026, 6, 2),
            new DateTime(2026, 6, 4));

        Assert.Equal(7, available); // 10 total - 3 booked
    }

    [Fact]
    public async Task Delete_SoftDeletesEquipment()
    {
        await using var db = CreateInMemoryDb();
        var category = new EquipmentCategory { Name = "Climbing" };
        db.EquipmentCategories.Add(category);
        await db.SaveChangesAsync();

        var equipment = new Equipment
        {
            Name = "Harness",
            CategoryId = category.Id,
            DailyRate = 20,
            TotalUnits = 8,
            Status = EquipmentStatus.Active
        };
        db.Equipment.Add(equipment);
        await db.SaveChangesAsync();

        var repo = new EquipmentRepository(db);
        repo.Delete(equipment);
        await db.SaveChangesAsync();

        var found = await db.Equipment.FindAsync(equipment.Id);
        Assert.Equal(EquipmentStatus.Retired, found!.Status);
    }
}
