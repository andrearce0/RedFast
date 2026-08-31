using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Entities;
using RedFast.Modules.Core.Entities.Outbox;
using System.Reflection;

namespace RedFast.Modules.Core.Persistence;

public class RedFastDbContext : DbContext
{
    public RedFastDbContext(DbContextOptions<RedFastDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PackageEvent> PackageEvents => Set<PackageEvent>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Sender> Senders => Set<Sender>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
