using Hmcr.Data.Database.Entities;
using System.Threading.Tasks;

namespace Hmcr.Data.Database
{
    public interface IUnitOfWork
    {
        bool Commit();
        Task<bool> CommitAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Commit()
        {
            return _dbContext.SaveChanges() >= 0;
        }

        public async Task<bool> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync() >= 0;
        }
    }
}
