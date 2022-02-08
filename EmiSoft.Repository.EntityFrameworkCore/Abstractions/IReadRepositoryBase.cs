using System.Linq.Expressions;

namespace EmiSoft.Repository.EntityFrameworkCore.Abstractions;

/// <summary>
/// <para>
/// A <see cref="IReadRepositoryBase{T}" /> can be used to query instances of <typeparamref name="T" />.
/// </para>
/// </summary>
/// <typeparam name="T">The type of entity being operated on by this repository.</typeparam>
public interface IReadRepositoryBase<T> where T : class
{
    IQueryable<T> Table { get; }
    IQueryable<T> TableNoTracking { get; }
    int GetTempId(T entity);
    T Find(Expression<Func<T, bool>> match);
    ICollection<T> FindAll(Expression<Func<T, bool>> match);
    Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match, CancellationToken cancellationToken = default);
    IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
    Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> match, CancellationToken cancellationToken = default);
    T Get(object id);
    IQueryable<T> GetAll();
    Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> GetAsync(object id, CancellationToken cancellationToken = default);
    IQueryable<T> IncludeMany(params Expression<Func<T, object>>[] includes);
    IQueryable<T> IncludeMany(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
}
