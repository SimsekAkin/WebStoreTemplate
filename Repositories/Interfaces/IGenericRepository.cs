namespace WebStore.Repositories.Interfaces;

/// <summary>
/// Generic repository contract that provides basic CRUD operations for any entity type.
/// Implement this interface (or extend <see cref="GenericRepository{TEntity}"/>) to create
/// entity-specific repositories without rewriting boilerplate.
/// </summary>
/// <typeparam name="TEntity">The EF Core entity class managed by this repository.</typeparam>
public interface IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>Returns all rows for <typeparamref name="TEntity"/> without tracking.</summary>
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns a single entity by its integer primary key, or <c>null</c> if not found.</summary>
    /// <param name="id">Primary key value.</param>
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Stages a new entity for insertion. Call <c>SaveChangesAsync</c> to persist.</summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages an existing entity for update.
    /// Returns <c>true</c> if changes were detected and staged; <c>false</c> if the entity was
    /// already <see cref="EntityState.Unchanged"/> — no SQL will be generated in that case.
    /// The caller can use the return value to show a "Nothing was changed." message to the user.
    /// Call <c>SaveChangesAsync</c> to persist when <c>true</c> is returned.
    /// </summary>
    bool Update(TEntity entity);

    /// <summary>
    /// Looks up the entity by <paramref name="keyValues"/> and, if found, marks it for deletion.
    /// Call <c>SaveChangesAsync</c> to persist.
    /// </summary>
    Task DeleteByIdAsync(object[] keyValues, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists all staged changes (Add/Update/Delete) to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
