using Microsoft.EntityFrameworkCore;
using WebStore.Models;

namespace WebStore.Data;

/// <summary>
/// Main EF Core database context for WebStore.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Creates a new context instance.
    /// </summary>
    /// <param name="options">DbContext configuration options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Product table.
    /// </summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// Configure model rules and seed data.
    /// In exam, add your relationships and keys here.
    /// Example:
    /// public DbSet<Raum> Raum { get; set; }
    /// public DbSet<Produkt> Produkt { get; set; }
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // In exam: add composite keys and relationships here.
        // Example:
        // modelBuilder.Entity<BuchungAusstattung>()
        //     .HasKey(ba => new { ba.BuchungId, ba.AusstattungsvarianteId });

        // Central seed registration point.
        SeedData.Seed(modelBuilder);
    }
}
