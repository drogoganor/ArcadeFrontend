using ArcadeFrontend.Sqlite.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArcadeFrontend.Sqlite.Configuration;

public class MameRomConfiguration : IEntityTypeConfiguration<MameRom>
{
    public void Configure(EntityTypeBuilder<MameRom> builder)
    {
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Title).IsRequired().HasMaxLength(500);
    }
}
