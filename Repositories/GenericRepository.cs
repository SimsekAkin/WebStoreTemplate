using Microsoft.EntityFrameworkCore;
using WebStore.Data;
using WebStore.Repositories.Interfaces;

namespace WebStore.Repositories;

/// <summary>
/// Generic EF Core repository for common CRUD operations.
/// This class is the shared base for entity-specific repositories such as ProductRepository.
/// It centralizes repeated data-access logic so controllers stay clean and focused.
/// </summary>
/// <typeparam name="TEntity">The EF Core entity class managed by this repository.</typeparam>
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>The database context shared across all repositories in the same request scope.</summary>
    protected readonly AppDbContext Context;

    /// <summary>The EF Core <see cref="DbSet{TEntity}"/> for <typeparamref name="TEntity"/>.</summary>
    protected readonly DbSet<TEntity> DbSet;

    /// <summary>Initialises the repository and resolves the correct <see cref="DbSet{TEntity}"/>.</summary>
    public GenericRepository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    /// <summary>
    /// Reads all rows of <typeparamref name="TEntity"/> as a read-only list.
    /// </summary>
    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // AsNoTracking is used because this is a read-only list query.
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Finds one entity by its integer primary key.
    /// Returns null when no matching row exists.
    /// </summary>
    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <summary>
    /// Adds a new entity to the current DbContext change tracker.
    /// Changes are persisted only after SaveChangesAsync is called.
    /// </summary>
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Marks an entity as modified and returns whether an update is needed.
    /// Returns false when EF already sees no changes.
    /// </summary>
    public bool Update(TEntity entity)
    {
        var entry = Context.Entry(entity);

        // EF Core is already tracking this object and found zero differences.
        // Returning false lets the caller show a "Nothing was changed." message to the user.
        if (entry.State == EntityState.Unchanged)
            return false;

        // Not unchanged: mark as modified and let SaveChangesAsync persist the update.
        DbSet.Update(entity);
        return true;
    }

    /// <summary>
    /// Deletes an entity by key values when found.
    /// If key does not match any row, method exits without throwing.
    /// </summary>
    public async Task DeleteByIdAsync(object[] keyValues, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync(keyValues, cancellationToken);
        if (entity is not null)
        {
            DbSet.Remove(entity);
        }
    }

    /// <summary>
    /// Persists all pending tracked changes in this DbContext.
    /// Returns affected row count from EF Core.
    /// </summary>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }
}
