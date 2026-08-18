using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedFast.Modules.Core.Entities;

namespace RedFast.Modules.Core.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.LicensePlate).IsRequired().HasMaxLength(10);
        builder.HasIndex(v => v.LicensePlate).IsUnique();

        builder.Property(v => v.Model).IsRequired().HasMaxLength(50);

        builder.Property(v => v.MaxCapacityKg).HasPrecision(8, 2);
    }
}
