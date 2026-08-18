using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedFast.Modules.Core.Entities;

namespace RedFast.Modules.Core.Persistence.Configurations;

public class PackageEventConfiguration : IEntityTypeConfiguration<PackageEvent>
{
    public void Configure(EntityTypeBuilder<PackageEvent> builder)
    {
        builder.ToTable("PackageEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Location).HasMaxLength(150);

        builder.HasOne<Package>()
            .WithMany(p => p.Events)
            .HasForeignKey(e => e.PackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
