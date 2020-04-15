using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Dtos.SubmissionStream;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface ISubmissionStreamRepository
    {
        Task<IEnumerable<SubmissionStreamDto>> GetSubmissionStreamsAsync(bool? isActive = true);
        Task<SubmissionStreamDto> GetSubmissionStreamByTableNameAsync(string tableName);
    }

    public class SubmissionStreamRepository : HmcrRepositoryBase<HmrSubmissionStream>, ISubmissionStreamRepository
    {
        public SubmissionStreamRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<SubmissionStreamDto>> GetSubmissionStreamsAsync(bool? isActive = true)
        {
            var query = DbSet.AsNoTracking();

            if (isActive != null)
            {
                if ((bool)isActive)
                {
                    query = query.Where(x => x.EndDate == null || x.EndDate > DateTime.Today);
                }
                else
                {
                    query = query.Where(x => x.EndDate != null && x.EndDate <= DateTime.Today);
                }
            }

            var reportTypes = Mapper.Map<IEnumerable<SubmissionStreamDto>>(await query.ToListAsync());
            
            foreach(var reportType in reportTypes)
            {
                if (reportType.FileSizeLimit == null)
                    reportType.FileSizeLimit = Constants.MaxFileSize;
            }

            return reportTypes;
        }

        public async Task<SubmissionStreamDto> GetSubmissionStreamByTableNameAsync(string tableName)
        {
            var stream = await DbSet.AsNoTracking().FirstAsync(x => x.StagingTableName == tableName);

            var reportType = Mapper.Map<SubmissionStreamDto>(stream);

            if (reportType.FileSizeLimit == null)
                reportType.FileSizeLimit = Constants.MaxFileSize;

            return reportType;
        }
    }
}
