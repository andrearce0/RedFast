using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedFast.Modules.Core.Entities;

namespace RedFast.Modules.Core.Persistence.Configurations;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.ToTable("Packages");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TrackingCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(p => p.TrackingCode).IsUnique();

        builder.Property(p => p.OriginAddress)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(p => p.DestinationAddress)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(p => p.Weight)
            .HasPrecision(10, 2);

        builder.HasOne(p => p.Sender)
            .WithMany(s => s.Packages)
            .HasForeignKey(p => p.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Driver)
            .WithMany(d => d.Packages)
            .HasForeignKey(p => p.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Metadata.FindNavigation(nameof(Package.Events))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
