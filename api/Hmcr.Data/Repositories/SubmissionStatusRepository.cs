using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.SubmissionStatus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface ISubmissionStatusRepository
    {
        Task<decimal> GetStatusIdByTypeAndCodeAsync(string type, string code);
        Task<IEnumerable<SubmissionStatusDto>> GetActiveStatuses();
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
        
        public async Task<IEnumerable<SubmissionStatusDto>> GetActiveStatuses()
        {
            return await GetAllAsync<SubmissionStatusDto>();
        }
    }
}
