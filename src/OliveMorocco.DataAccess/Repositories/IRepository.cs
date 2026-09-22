using System.Linq.Expressions;
using OliveMorocco.Domain.Common;

namespace OliveMorocco.DataAccess.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<T?> GetByIdWithNavigationsAsync(
        int id,
        Expression<Func<T, object>>[] includes,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> FindWithIncludesAsync(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>[] includes,
        CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<TResult> Items, int TotalCount)> QueryPagedAsync<TResult>(
        Expression<Func<T, bool>>? predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        Expression<Func<T, TResult>> selector,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
