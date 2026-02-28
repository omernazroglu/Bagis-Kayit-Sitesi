using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Linq;
using System.Runtime.CompilerServices;
using UFUKDER_BAGIS.Models;
using UFUKDER_BAGIS.Services.Interfaces;

namespace UFUKDER_BAGIS.Services.Concrete
{
    public class RepositoryAsync<T> : IRepositoryAsync<T> where T : class
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public RepositoryAsync(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();

        }

        public RepositoryAsync()
        {
        }

        public virtual async Task<Result<T>> GetItemAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>>? include = null)
        {
            try
            {
                var query = _dbSet.AsNoTracking();
                if (predicate != null) query = query.Where(predicate);
                if (include != null) query = query.Include(include);
                var item = await query.FirstOrDefaultAsync();
                if (item == null) return Result<T>.Failure("item null");
                return Result<T>.Success(item);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }

        public virtual async Task<Result<T>> GetItemWithoutBlobsAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>>? include = null)
        {
            try
            {
                var query = _dbSet.AsNoTracking();
                if (predicate != null) query = query.Where(predicate);
                if (include != null) query = query.Include(include);
                var properties = typeof(T).GetProperties()
                                          .Where(p => !p.PropertyType.Equals(typeof(byte[])) // BLOB olmayanları seç
                                                        && !p.GetCustomAttributes(typeof(ColumnAttribute), false)
                                                             .Cast<ColumnAttribute>()
                                                             .Any(attr => attr.TypeName == "BLOB"))
                                          .ToList();

                var parameter = Expression.Parameter(typeof(T), "x");
                var bindings = properties.Select(prop =>
                    Expression.Bind(
                        prop,
                        Expression.Property(parameter, prop.Name)
                    )).ToList();
                var body = Expression.MemberInit(Expression.New(typeof(T)), bindings);
                var selector = Expression.Lambda<Func<T, T>>(body, parameter);
                var list = await query.Select(selector).FirstOrDefaultAsync();

                if (list == null)
                    return Result<T>.Failure("list null");

                return Result<T>.Success(list);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }

        public async Task<Result<T>> InsertAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                //await _dbContext.SaveChangesAsync();
                return Result<T>.Success(entity);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }

        public async Task<Result<IEnumerable<T>>> InsertRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                await _dbSet.AddRangeAsync(entities);
                await _dbContext.SaveChangesAsync();
                return Result<IEnumerable<T>>.Success(entities);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<T>>.Failure(ex.Message);
            }
        }

        public async Task<Result<IEnumerable<T>>> UpdateRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                _dbSet.UpdateRange(entities);
                await _dbContext.SaveChangesAsync();
                return Result<IEnumerable<T>>.Success(entities);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<T>>.Failure(ex.Message);
            }
        }

        public async Task<Result<T>> UpdateAsync(T entity)
        {
            try
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
                //await _dbContext.SaveChangesAsync();
                return Result<T>.Success(entity);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }

        public async Task<Result<T>> DeleteAsync(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                await _dbContext.SaveChangesAsync();
                return Result<T>.Success(entity);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }

        public async Task<Result<IEnumerable<T>>> DeleteRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
                await _dbContext.SaveChangesAsync();
                return Result<IEnumerable<T>>.Success(entities);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<T>>.Failure(ex.Message);
            }
        }


        public async Task<Result<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>[]? includes = null)
        {
            try
            {
                IQueryable<T> query = _dbSet;
                if (predicate != null) query = query.Where(predicate);
                //if (include != null) query = query.Include(include);
                if (includes != null)
                    foreach (var include in includes)
                        query = query.Include(include);
                var list = await query.ToListAsync();
                if (list == null) return Result<T>.Failure("list null");

                return Result<T>.Success(list);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }

        public async Task<Result<T>> GetListWithoutBlobsAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, object>>[]? includes = null)
        {
            try
            {
                IQueryable<T> query = _dbSet;
                if (predicate != null)
                    query = query.Where(predicate);

                if (includes != null)
                {
                    foreach (var include in includes)
                        query = query.Include(include);
                }
                var properties = typeof(T).GetProperties()
                                          .Where(p => !p.PropertyType.Equals(typeof(byte[])) // BLOB olmayanları seç
                                                        && !p.GetCustomAttributes(typeof(ColumnAttribute), false)
                                                             .Cast<ColumnAttribute>()
                                                             .Any(attr => attr.TypeName == "BLOB"))
                                          .ToList();

                var parameter = Expression.Parameter(typeof(T), "x");
                var bindings = properties.Select(prop =>
                    Expression.Bind(
                        prop,
                        Expression.Property(parameter, prop.Name)
                    )).ToList();
                var body = Expression.MemberInit(Expression.New(typeof(T)), bindings);
                var selector = Expression.Lambda<Func<T, T>>(body, parameter);
                var list = await query.Select(selector).ToListAsync();

                if (list == null)
                    return Result<T>.Failure("list null");

                return Result<T>.Success(list);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }

        public async Task<Result<T>> GetByIdAsync(long id)
        {
            try
            {
                var item = await _dbSet.FindAsync(id);
                if (item == null) return Result<T>.Failure("item null");
                return Result<T>.Success(item);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(ex.Message);
            }
        }



        public async Task<Result<byte[]>> GetFileAsync(string tableName, string guid, string columnName)
        {
            try
            {
                string query = $"SELECT {columnName} AS \"Value\" FROM {tableName} WHERE SATIRGUID = '{guid}'";

                var result = await _dbContext.Database.SqlQuery<byte[]>(FormattableStringFactory.Create(query)).FirstOrDefaultAsync();
                if (result == null) return Result<byte[]>.Failure("item null");

                return Result<byte[]>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<byte[]>.Failure(ex.Message);
            }
        }


    }

}
