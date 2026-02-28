using System.Linq.Expressions;
using UFUKDER_BAGIS.Services.Interfaces;
using UFUKDER_BAGIS.Models;
namespace UFUKDER_BAGIS.Services.Concrete
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly IRepositoryAsync<T> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public Service(IRepositoryAsync<T> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;

        }

        public async Task<Result<T>> GetByIdAsync(long id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Result<T>> GetItemAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>? include = null)
        {
            return await _repository.GetItemAsync(predicate, include);
        }
        public async Task<Result<T>> GetItemWithoutBlobsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>? include = null)
        {
            return await _repository.GetItemWithoutBlobsAsync(predicate, include);
        }

        public async Task<Result<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>[]? includes = null)
        {
            return await _repository.GetListAsync(predicate, includes);
        }

        public async Task<Result<T>> GetListWithoutBlobsAsync(Expression<Func<T, bool>>? predicate = null,
           Expression<Func<T, object>>[]? includes = null)
        {
            return await _repository.GetListWithoutBlobsAsync(predicate, includes);
        }


        public async Task<Result<T>> InsertAsync(T entity)
        {
            var result = await _repository.InsertAsync(entity);
            if (!result.IsSuccess) return result;

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }

        public async Task<Result<T>> InsertTransactionalAsync(T entity)
        {
            return await _repository.InsertAsync(entity);
        }

        public async Task<Result<T>> UpdateAsync(T entity)
        {
            var result = await _repository.UpdateAsync(entity);
            if (!result.IsSuccess) return result;

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }

        public async Task<Result<T>> UpdateTransactionalAsync(T entity)
        {
            return await _repository.UpdateAsync(entity);
        }


        public async Task<Result<T>> DeleteAsync(long id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (!result.IsSuccess || result.Item == null)
            {
                return Result<T>.Failure("Item not found.");
            }

            return await _repository.DeleteAsync(result.Item);
        }

        public async Task<Result<IEnumerable<T>>> InsertRangeAsync(IEnumerable<T> entities)
        {
            return await _repository.InsertRangeAsync(entities);
        }

        public async Task<Result<IEnumerable<T>>> UpdateRangeAsync(IEnumerable<T> entities)
        {
            return await _repository.UpdateRangeAsync(entities);
        }

        public async Task<Result<IEnumerable<T>>> DeleteRangeAsync(IEnumerable<T> entities)
        {
            return await _repository.DeleteRangeAsync(entities);
        }

    }

}
