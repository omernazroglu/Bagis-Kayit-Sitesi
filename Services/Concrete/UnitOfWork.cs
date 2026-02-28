using Microsoft.EntityFrameworkCore.Storage;
using UFUKDER_BAGIS.Models;
using UFUKDER_BAGIS.Services.Interfaces;

namespace UFUKDER_BAGIS.Services.Concrete
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_transaction == null)
                _transaction = await _dbContext.Database.BeginTransactionAsync();

            return _transaction;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _dbContext?.Dispose();
        }
    }
}
