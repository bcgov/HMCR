using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.WildlifeReport;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IWildlifeReportRepository
    {
        void SaveWildlifeReport(HmrSubmissionObject submission, List<WildlifeReportDto> rows);
        Task<IEnumerable<WildlifeReportExportDto>> ExporReportAsync(decimal submissionObjectId);
    }
    public class WildlifeReportRepository : HmcrRepositoryBase<HmrWildlifeReport>, IWildlifeReportRepository, IReportExportRepository<WildlifeReportExportDto>
    {
        public WildlifeReportRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public void SaveWildlifeReport(HmrSubmissionObject submission, List<WildlifeReportDto> rows)
        {
            foreach (var row in rows)
            {
                row.SubmissionObjectId = submission.SubmissionObjectId;

                submission.HmrWildlifeReports
                    .Add(Mapper.Map<HmrWildlifeReport>(row));
            }
        }
        public async Task<IEnumerable<WildlifeReportExportDto>> ExporReportAsync(decimal submissionObjectId)
        {
            var entities = await DbSet.AsNoTracking()
                .Include(x => x.SubmissionObject)
                    .ThenInclude(x => x.Party)
                .Where(x => x.SubmissionObjectId == submissionObjectId)
                .OrderBy(x => x.RowNum)
                .ToListAsync();

            return Mapper.Map<IEnumerable<WildlifeReportExportDto>>(entities);
        }
    }
}
