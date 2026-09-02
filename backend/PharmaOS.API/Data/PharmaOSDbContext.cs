using Microsoft.EntityFrameworkCore;
using PharmaOS.API.Models;

namespace PharmaOS.API.Data;

public class PharmaOSDbContext : DbContext
{
    public PharmaOSDbContext(DbContextOptions<PharmaOSDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Pharmacy> Pharmacies => Set<Pharmacy>();
    public DbSet<Drug> Drugs => Set<Drug>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
}
