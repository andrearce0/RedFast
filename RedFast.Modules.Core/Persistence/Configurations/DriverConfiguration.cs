using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedFast.Modules.Core.Entities;

namespace RedFast.Modules.Core.Persistence.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("Drivers");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).IsRequired().HasMaxLength(100);

        builder.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(20);
        builder.HasIndex(d => d.LicenseNumber).IsUnique();

        builder.HasOne(d => d.Vehicle)
            .WithMany()
            .HasForeignKey(d => d.VehicleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
