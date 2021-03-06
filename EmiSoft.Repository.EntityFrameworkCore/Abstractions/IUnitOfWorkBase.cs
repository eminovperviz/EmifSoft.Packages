namespace EmiSoft.Repository.EntityFrameworkCore.Abstractions;

public interface IUnitOfWorkBase
{
    int Commit();
    Task<int> CommitAsync();
    int ExecuteSqlRaw(string sqlQuery);
    Task<int> ExecuteSqlRawAsync(string sqlQuery);
    Task<int> ExecuteSqlInterpolatedAsync(FormattableString sqlQuery);
    void Rollback();
    Task RollbackAsync();
}
