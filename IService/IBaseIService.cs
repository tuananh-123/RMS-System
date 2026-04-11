using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace RMS.IService;

public interface IBaseService<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void UpdateAsync(T entity);
    void DeleteAsync(T entity);
    Task SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}
