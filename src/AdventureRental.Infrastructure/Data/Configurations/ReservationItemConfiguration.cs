using AdventureRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureRental.Infrastructure.Data.Configurations;

public class ReservationItemConfiguration : IEntityTypeConfiguration<ReservationItem>
{
    public void Configure(EntityTypeBuilder<ReservationItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.DailyRate).HasPrecision(18, 2);
        builder.Property(i => i.LineTotal).HasPrecision(18, 2);

        builder.HasOne(i => i.Reservation)
            .WithMany(r => r.Items)
            .HasForeignKey(i => i.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Equipment)
            .WithMany(e => e.ReservationItems)
            .HasForeignKey(i => i.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
