using System.Linq.Expressions;
using UFUKDER_BAGIS.Models;
namespace UFUKDER_BAGIS.Services.Interfaces
{
    public interface IRepositoryAsync<T> where T : class
    {
        Task<Result<T>> GetByIdAsync(long id);
        Task<Result<T>> GetItemAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>? include = null);
        Task<Result<T>> GetItemWithoutBlobsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>? include = null);
        Task<Result<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, object>>[]? include = null);
        Task<Result<T>> GetListWithoutBlobsAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, object>>[]? include = null);
        Task<Result<T>> InsertAsync(T entity);
        Task<Result<T>> UpdateAsync(T entity);
        Task<Result<T>> DeleteAsync(T entity);
        Task<Result<IEnumerable<T>>> DeleteRangeAsync(IEnumerable<T> entities);
        Task<Result<IEnumerable<T>>> InsertRangeAsync(IEnumerable<T> entities);
        Task<Result<IEnumerable<T>>> UpdateRangeAsync(IEnumerable<T> entities);

    }
}
