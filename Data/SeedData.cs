using Microsoft.EntityFrameworkCore;

namespace WebStore.Data;

/// <summary>
/// Seed data for initial database population.
/// Used during application startup to populate test data.
/// </summary>
public static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // SINAVDA: HasData kullanarak seed kayıtlarını burada tanımla.
        // Örnek:
        // modelBuilder.Entity<Category>().HasData(
        //     new Category { Id = 1, Name = "Elektronik" },
        //     new Category { Id = 2, Name = "Kitap" }
        // );
    }
}
