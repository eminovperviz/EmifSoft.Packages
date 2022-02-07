using EmiSoft.Repository.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace EmiSoft.Repository.EntityFrameworkCore;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    public EfUnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public int ExecuteSqlRaw(string sqlQuery) => _dbContext.Database.ExecuteSqlRaw(sqlQuery);
    public Task<int> ExecuteSqlInterpolatedAsync(FormattableString sqlQuery) => _dbContext.Database.ExecuteSqlInterpolatedAsync(sqlQuery);

    public async Task<int> ExecuteSqlRawAsync(string sqlQuery) => await _dbContext.Database.ExecuteSqlRawAsync(sqlQuery);

    public int Commit() => _dbContext.SaveChanges(true);

    public async Task<int> CommitAsync() => await _dbContext.SaveChangesAsync();

    public void Rollback() => _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());

    public async Task RollbackAsync()
    {
        foreach (var entity in _dbContext.ChangeTracker.Entries())
        {
            await entity.ReloadAsync();
        }
    }
}
