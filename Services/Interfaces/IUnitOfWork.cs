using Microsoft.EntityFrameworkCore.Storage;
using UFUKDER_BAGIS.Models;

namespace UFUKDER_BAGIS.Services.Interfaces
{
    public interface IUnitOfWork
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> SaveChangesAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }

}
