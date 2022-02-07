using EmiSoft.Domain;
using EmiSoft.Repository.EntityFrameworkCore.Abstractions;
using EmiSoft.Repository.EntityFrameworkCore.Utility;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EmiSoft.Repository.EntityFrameworkCore;

public class EfRepository<T> : IRepository<T> where T : BaseEntity<int>
{
    public int Culture
    {
        get
        {
            string culture = Thread.CurrentThread.CurrentCulture.Name;

            return LanguageOperation.Find(culture).Value;
        }
    }

    private readonly ApplicationDbContext _context;
    private DbSet<T> _dbSet;
    public virtual IQueryable<T> Table => _dbSet;
    public virtual IQueryable<T> TableNoTracking => _dbSet.AsNoTracking();

    public EfRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity, bool isCommit = true, CancellationToken cancellationToken = new CancellationToken())
    {
        await _dbSet.AddAsync(entity, cancellationToken);

        await SaveChangesAsync(isCommit, cancellationToken);
    }

    public int GetTempId(T entity)
    {
        return _context.Entry(entity).Property(e => e.Id).CurrentValue;
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, bool isCommit = true)
    {
        _dbSet.AddRange(entities);
        await SaveChangesAsync(isCommit);
    }

    public void Attach(T entity) => _dbSet.Attach(entity);

    public void AttachRange(IEnumerable<T> entities) => _dbSet.AttachRange(entities);

    public async Task DeleteAsync(T entity, bool isCommit = true)
    {
        _dbSet.Remove(entity);
        await SaveChangesAsync(isCommit);
    }

    public async Task DeleteAsync(T entity, bool isCommit = true, CancellationToken cancellationToken = new CancellationToken())
    {
        _dbSet.Remove(entity);
        await SaveChangesAsync(isCommit, cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, bool isCommit = true)
    {
        _dbSet.RemoveRange(entities);
        await SaveChangesAsync(isCommit);
    }

    public async Task UpdateAsync(T entity, bool isCommit = true, CancellationToken cancellationToken = new CancellationToken())
    {
        _dbSet.Update(entity);
        await SaveChangesAsync(isCommit, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(bool isCommit = true, CancellationToken cancellationToken = new CancellationToken()) => await _context.SaveChangesAsync(isCommit, cancellationToken);

    public T Find(Expression<Func<T, bool>> match) => Table.FirstOrDefault(match);

    public ICollection<T> FindAll(Expression<Func<T, bool>> match) => Table.Where(match).ToList();

    /// <summary>
    /// Don't use multiple async/await operation on same context. SEE link
    /// https://aka.ms/efcore-docs-threading
    /// </summary>
    /// <returns></returns>
    public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match, CancellationToken cancellationToken = default)
    {
        var result = await Table.Where(match).ToListAsync(cancellationToken);
        return result;
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> match, CancellationToken cancellationToken = default)
    {
        var result = await Table.FirstOrDefaultAsync(match, cancellationToken);
        return result;
    }

    public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate) => Table.Where(predicate);

    public async Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var result = await Table.Where(predicate).ToListAsync(cancellationToken);
        return result;
    }

    public T Get(object id) => _dbSet.Find(id);

    public IQueryable<T> GetAll() => Table;

    public async Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await Table.ToListAsync(cancellationToken);
        return result;
    }

    public async Task<T> GetAsync(object id, CancellationToken cancellationToken = default)
    {
        var result = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        return result;
    }

    public IQueryable<T> IncludeMany(params Expression<Func<T, object>>[] includes)
    {
        if (includes != null)
        {
            _dbSet = includes.Aggregate(_dbSet, (current, include) => (DbSet<T>)current.Include(include));
        }
        return _dbSet;
    }

    public IQueryable<T> IncludeMany(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        if (includes != null)
        {
            query = includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        return query.Where(predicate);
    }

    public async Task UpdateSpecialPropertiesAsync(T entity, bool isCommit = true, params Expression<Func<T, object>>[] properties)
    {
        var entry = _context.Entry(entity);

        entry.State = EntityState.Unchanged;

        foreach (var prop in properties)
        {
            entry.Property(prop).IsModified = true;
        }

        await SaveChangesAsync(isCommit);
    }

    public async Task UpdateExceptedPropertiesAsync(T entity, bool isCommit = false, params Expression<Func<T, object>>[] properties)
    {
        var entry = _context.Entry(entity);

        entry.State = EntityState.Modified;

        foreach (var prop in properties)
        {
            entry.Property(prop).IsModified = false;
        }

        await SaveChangesAsync(isCommit);
    }
}

