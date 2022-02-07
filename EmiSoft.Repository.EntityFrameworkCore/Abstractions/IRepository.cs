using System.Linq.Expressions;

namespace EmiSoft.Repository.EntityFrameworkCore.Abstractions;

/// <summary>
/// <para>
/// A <see cref="IRepository{T}" /> can be used to query and save instances of <typeparamref name="T" />.
/// </para>
/// </summary>
/// <typeparam name="T">The type of entity being operated on by this repository.</typeparam>
public interface IRepository<T> : IReadRepository<T> where T : class
{
    Task AddAsync(T entity, bool isCommit = true, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, bool isCommit = true);
    void Attach(T entity);
    void AttachRange(IEnumerable<T> entities);
    Task DeleteAsync(T entity, bool isCommit = true, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, bool isCommit = true);
    Task UpdateAsync(T entity, bool isCommit = true, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(bool isCommit = true, CancellationToken cancellationToken = new CancellationToken());

    Task UpdateSpecialPropertiesAsync(T entity, bool isCommit = true, params Expression<Func<T, object>>[] properties);
    Task UpdateExceptedPropertiesAsync(T entity, bool isCommit = false, params Expression<Func<T, object>>[] properties);
}
