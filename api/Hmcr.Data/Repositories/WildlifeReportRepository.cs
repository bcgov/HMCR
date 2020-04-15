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
        void SaveWildlifeReport(HmrSubmissionObject submission, List<WildlifeReportGeometry> rows);
        Task<IEnumerable<WildlifeReportExportDto>> ExportReportAsync(decimal submissionObjectId);
    }
    public class WildlifeReportRepository : HmcrRepositoryBase<HmrWildlifeReport>, IWildlifeReportRepository, IReportExportRepository<WildlifeReportExportDto>
    {
        public WildlifeReportRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public void SaveWildlifeReport(HmrSubmissionObject submission, List<WildlifeReportGeometry> rows)
        {
            foreach (var row in rows)
            {
                row.WildlifeReportTyped.SubmissionObjectId = submission.SubmissionObjectId;
                
                var entity = Mapper.Map<HmrWildlifeReport>(row.WildlifeReportTyped);
                entity.Geometry = row.Geometry;

                submission.HmrWildlifeReports
                    .Add(entity);
            }
        }
        public async Task<IEnumerable<WildlifeReportExportDto>> ExportReportAsync(decimal submissionObjectId)
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
