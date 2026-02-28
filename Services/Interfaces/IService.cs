using System.Linq.Expressions;
using UFUKDER_BAGIS.Models;
namespace UFUKDER_BAGIS.Services.Interfaces
{
    public interface IService<T> where T : class
    {

        Task<Result<T>> GetByIdAsync(long id);
        Task<Result<T>> GetItemAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>? include = null);
        Task<Result<T>> GetItemWithoutBlobsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>? include = null);
        Task<Result<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, object>>[]? include = null);
        Task<Result<T>> GetListWithoutBlobsAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, object>>[]? include = null);
        //Task<Result<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[]? include);
        Task<Result<T>> InsertAsync(T entity);

        Task<Result<T>> InsertTransactionalAsync(T entity);
        Task<Result<T>> UpdateAsync(T entity);
        Task<Result<T>> UpdateTransactionalAsync(T entity);
        Task<Result<T>> DeleteAsync(long id);

        Task<Result<IEnumerable<T>>> InsertRangeAsync(IEnumerable<T> entities);
        Task<Result<IEnumerable<T>>> UpdateRangeAsync(IEnumerable<T> entities);
        Task<Result<IEnumerable<T>>> DeleteRangeAsync(IEnumerable<T> entities);
    }

}
