using AdventureRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureRental.Infrastructure.Data.Configurations;

public class EquipmentCategoryConfiguration : IEntityTypeConfiguration<EquipmentCategory>
{
    public void Configure(EntityTypeBuilder<EquipmentCategory> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.Property(c => c.IconUrl).HasMaxLength(500);
    }
}
