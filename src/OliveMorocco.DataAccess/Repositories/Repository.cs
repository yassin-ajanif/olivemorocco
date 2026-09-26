using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Domain.Common;

namespace OliveMorocco.DataAccess.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Db;
    protected DbSet<T> Set => Db.Set<T>();

    public Repository(AppDbContext db)
    {
        Db = db;
    }

    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Set.FindAsync([id], cancellationToken).AsTask();

    
    public async Task<T?> GetByIdWithNavigationsAsync(
        int id,
        Expression<Func<T, object>>[] includes,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = Set;
        foreach (var include in includes)
            query = query.Include(include);

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Set.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await Set.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        => Set.AsNoTracking().AnyAsync(predicate, cancellationToken);

    public async Task<IReadOnlyList<T>> FindWithIncludesAsync(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>[] includes,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = Set.AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);

        return await query.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<TResult> Items, int TotalCount)> QueryPagedAsync<TResult>(
        Expression<Func<T, bool>>? predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        Expression<Func<T, TResult>> selector,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 15;

        IQueryable<T> query = Set.AsNoTracking();
        if (predicate is not null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await orderBy(query)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        Set.Add(entity);
        await Db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (Db.Entry(entity).State == EntityState.Detached)
            Set.Update(entity);

        await Db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Set.FindAsync([id], cancellationToken);
        if (entity is null)
            return;

        Set.Remove(entity);
        await Db.SaveChangesAsync(cancellationToken);
    }

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await Db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await action(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
