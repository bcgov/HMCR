using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.SubmissionStatus;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface ISubmissionStatusRepository
    {
        Task<decimal> GetStatusIdByTypeAndCodeAsync(string type, string code);
        decimal GetStatusIdByTypeAndCode(string type, string code);
        Task<IEnumerable<SubmissionStatusDto>> GetActiveStatusesAsync();
    }
    public class SubmissionStatusRepository : HmcrRepositoryBase<HmrSubmissionStatu>, ISubmissionStatusRepository
    {
        public SubmissionStatusRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<decimal> GetStatusIdByTypeAndCodeAsync(string type, string code)
        {
            return (await DbSet.FirstAsync(x => x.StatusCode == code && x.StatusType == type)).StatusId;
        }

        public decimal GetStatusIdByTypeAndCode(string type, string code)
        {
            return GetFirst<SubmissionStatusDto>(x => x.StatusCode == code && x.StatusType == type).StatusId;
        }

        public async Task<IEnumerable<SubmissionStatusDto>> GetActiveStatusesAsync()
        {
            return await GetAllAsync<SubmissionStatusDto>();
        }
    }
}
