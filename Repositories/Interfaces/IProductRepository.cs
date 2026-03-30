using WebStore.Models;

namespace WebStore.Repositories.Interfaces;

/// <summary>
/// Product-specific repository contract.
/// Extends <see cref="IGenericRepository{TEntity}"/> with strongly-typed CRUD helpers
/// (using <c>int</c> ids) and product-specific query methods.
/// </summary>
public interface IProductRepository : IGenericRepository<Product>
{
    // ── Typed CRUD helpers ────────────────────────────────────────────────────

    /// <summary>Returns all products without change tracking.</summary>
    Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns, the product with the given <paramref name="id"/>, or <c>null</c> if not found.</summary>
    Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Adds a new product and persists it to the database.</summary>
    Task AddProductAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing product and persists the change.
    /// Returns <c>true</c> if a change was saved; <c>false</c> if nothing changed.
    /// </summary>
    Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the product by <paramref name="id"/> and persists the change.
    /// Returns <c>true</c> if the product existed and was deleted.
    /// </summary>
    Task<bool> DeleteProductByIdAsync(int id, CancellationToken cancellationToken = default);

    // ── Product-specific queries ──────────────────────────────────────────────

    /// <summary>
    /// Returns all products whose <c>Name</c> contains <paramref name="term"/> (case-insensitive),
    /// ordered alphabetically.
    /// </summary>
    Task<List<Product>> SearchByNameAsync(string term, CancellationToken cancellationToken = default);
}
