using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedFast.Modules.Core.Entities;

namespace RedFast.Modules.Core.Persistence.Configurations;

public class SenderConfiguration : IEntityTypeConfiguration<Sender>
{
    public void Configure(EntityTypeBuilder<Sender> builder)
    {
        builder.ToTable("Senders");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.CompanyName).IsRequired().HasMaxLength(150);

        builder.Property(s => s.Document).IsRequired().HasMaxLength(14);
        builder.HasIndex(s => s.Document).IsUnique();

        builder.HasIndex(s => s.UserId).IsUnique();
    }
}
