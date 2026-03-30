using Microsoft.EntityFrameworkCore;
using WebStore.Data;
using WebStore.Models;
using WebStore.Repositories.Interfaces;

namespace WebStore.Repositories;

/// <summary>
/// Product repository implementation.
/// Uses GenericRepository for common CRUD and adds product-specific methods.
/// </summary>
public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    /// <summary>
    /// Creates product repository with injected AppDbContext.
    /// </summary>
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    // Typed CRUD helper methods

    /// <summary>
    /// Returns all products.
    /// </summary>
    public Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        return GetAllAsync(cancellationToken);// Retrieves all products without tracking for read-only performance.
    }

    /// <summary>
    /// Returns one product by id. Null when not found.
    /// </summary>
    public Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return GetByIdAsync(id, cancellationToken);//Retrieves a product by its id using the generic repository method.
    }

    /// <summary>
    /// Adds a new product and saves changes immediately.
    /// </summary>
    public async Task AddProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        await AddAsync(product, cancellationToken);// Adds the new product to the DbSet 
        await SaveChangesAsync(cancellationToken);// Persist insert to database.
    }

    /// <summary>
    /// Updates a product and returns false if no value changed.
    /// </summary>
    public async Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        // If entity has no modifications, skip DB write.
        if (Context.Entry(product).State == EntityState.Unchanged)// Check if the product is already unchanged in the DbContext.
            return false;

        var changed = Update(product);//If the product was already unchanged, we return false to indicate that no update is needed.
        if (!changed)
            return false;//With no changes, we can return false.

        await SaveChangesAsync(cancellationToken);// Persist update to database.
        return true;
    }

    /// <summary>
    /// Deletes product by id. Returns false if product does not exist.
    /// </summary>
    public async Task<bool> DeleteProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // First check if product exists.
        var existing = await GetProductByIdAsync(id, cancellationToken);// Look up the product to delete first to ensure it exists
        if (existing is null)
            return false;//if there is no product with the specified id, return false 

        await DeleteByIdAsync(new object[] { id }, cancellationToken);//Delete the product by id using the generic repository method
        await SaveChangesAsync(cancellationToken);// Persist deletion to database.
        return true;
    }

    // Product-specific query methods

    /// <summary>
    /// Searches products by name and returns sorted results.
    /// </summary>
    public Task<List<Product>> SearchByNameAsync(string term, CancellationToken cancellationToken = default)
    {   
        term = term.Trim();// Remove accidental leading/trailing spaces from search text.

        return Context.Products
            .AsNoTracking() // AsNoTracking improves read-only query performance.
            .Where(p => p.Name.Contains(term))// Filter products whose name contains the search term.
            .OrderBy(p => p.Name)// Sort results alphabetically by name.
            .ToListAsync(cancellationToken);
    }
}
