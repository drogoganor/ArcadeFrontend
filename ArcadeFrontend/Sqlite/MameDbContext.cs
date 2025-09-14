using ArcadeFrontend.Sqlite.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArcadeFrontend.Sqlite;

public class MameDbContext : DbContext
{
    public DbSet<MameRom> MameRom { get; set; }

    public string DbPath { get; }

    public MameDbContext()
    {
        var path = Environment.CurrentDirectory;
        DbPath = Path.Join(path, "Content\\mame.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from the assembly where MameDbContext resides
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MameDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
