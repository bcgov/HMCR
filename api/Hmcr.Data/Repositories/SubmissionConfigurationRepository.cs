using Hmcr.Data.Database.Entities;
using Hmcr.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface ISubmissionConfigurationRepository
    {
        Task<IReadOnlyList<HmrSubmissionConfiguration>> GetAllAsync();
        Task<IReadOnlyList<HmrSubmissionConfiguration>> GetApplicableAsync(
            decimal submissionStreamId,
            decimal? serviceAreaNumber,
            bool activeOnly = true);
        Task<HmrSubmissionConfiguration> GetByIdForUpdateAsync(decimal submissionConfigurationId);
    }

    public class SubmissionConfigurationRepository : ISubmissionConfigurationRepository
    {
        private readonly AppDbContext _dbContext;

        public SubmissionConfigurationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<HmrSubmissionConfiguration>> GetAllAsync()
        {
            return await ConfigurationGraph(_dbContext.HmrSubmissionConfigurations.AsNoTracking())
                .OrderBy(x => x.ConfigurationName)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<HmrSubmissionConfiguration>> GetApplicableAsync(
            decimal submissionStreamId,
            decimal? serviceAreaNumber,
            bool activeOnly = true)
        {
            IQueryable<HmrSubmissionConfiguration> query = _dbContext.HmrSubmissionConfigurations
                .AsNoTracking()
                .Where(x => x.SubmissionStreamId == submissionStreamId);

            if (activeOnly)
            {
                query = query.Where(x => x.IsActive);
            }

            query = serviceAreaNumber.HasValue
                ? query.Where(x =>
                    x.ScopeType == SubmissionConfigurationValues.GlobalScope ||
                    (x.ScopeType == SubmissionConfigurationValues.ServiceAreaScope &&
                     x.ServiceAreas.Any(serviceArea =>
                         serviceArea.ServiceAreaNumber == serviceAreaNumber.Value)))
                : query.Where(x => x.ScopeType == SubmissionConfigurationValues.GlobalScope);

            return await ConfigurationGraph(query)
                .OrderBy(x => x.ConfigurationName)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<HmrSubmissionConfiguration> GetByIdForUpdateAsync(decimal submissionConfigurationId)
        {
            return await ConfigurationGraph(_dbContext.HmrSubmissionConfigurations)
                .AsSplitQuery()
                .SingleOrDefaultAsync(x => x.SubmissionConfigurationId == submissionConfigurationId);
        }

        private static IQueryable<HmrSubmissionConfiguration> ConfigurationGraph(
            IQueryable<HmrSubmissionConfiguration> query)
        {
            return query
                .Include(x => x.SubmissionStream)
                .Include(x => x.Windows)
                .Include(x => x.Rules)
                    .ThenInclude(x => x.Activities)
                        .ThenInclude(x => x.ActivityCode)
                .Include(x => x.Audiences)
                .Include(x => x.ServiceAreas);
        }
    }
}
