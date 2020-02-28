using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.RockfallReport;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IRockfallReportRepository
    {
        IAsyncEnumerable<HmrRockfallReport> SaveRockfallReportAsnyc(HmrSubmissionObject submission, List<RockfallReportGeometry> rows);
        Task<IEnumerable<RockfallReportExportDto>> ExportReportAsync(decimal submissionObjectId);
    }
    public class RockfallReportRepository : HmcrRepositoryBase<HmrRockfallReport>, IRockfallReportRepository, IReportExportRepository<RockfallReportExportDto>
    {
        public RockfallReportRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async IAsyncEnumerable<HmrRockfallReport> SaveRockfallReportAsnyc(HmrSubmissionObject submission, List<RockfallReportGeometry> rockfallReports)
        {
            foreach (var rockfallReport in rockfallReports)
            {
                rockfallReport.RockfallReportTyped.SubmissionObjectId = submission.SubmissionObjectId;

                var entity = await DbSet
                    .Where(x => x.SubmissionObject.PartyId == submission.PartyId && x.McrrIncidentNumber == rockfallReport.RockfallReportTyped.McrrIncidentNumber)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    entity = Mapper.Map<HmrRockfallReport>(rockfallReport.RockfallReportTyped);
                    entity.Geometry = rockfallReport.Geometry;

                    submission.HmrRockfallReports
                        .Add(entity);
                }
                else
                {
                    rockfallReport.RockfallReportTyped.RockfallReportId = entity.RockfallReportId;
                    Mapper.Map(rockfallReport.RockfallReportTyped, entity);
                    entity.Geometry = rockfallReport.Geometry;
                }

                yield return entity;
            }
        }

        public async Task<IEnumerable<RockfallReportExportDto>> ExportReportAsync(decimal submissionObjectId)
        {
            var entities = await DbSet.AsNoTracking()
                .Include(x => x.SubmissionObject)
                    .ThenInclude(x => x.Party)
                .Where(x => x.SubmissionObjectId == submissionObjectId)
                .OrderBy(x => x.RowNum)
                .ToListAsync();

            return Mapper.Map<IEnumerable<RockfallReportExportDto>>(entities);
        }
    }
}
