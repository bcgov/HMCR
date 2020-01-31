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
        IAsyncEnumerable<HmrRockfallReport> SaveRockfallReportAsnyc(HmrSubmissionObject submission, List<RockfallReportDto> rows);
    }
    public class RockfallReportRepository : HmcrRepositoryBase<HmrRockfallReport>, IRockfallReportRepository, IReportExportRepository<RockfallReportExportDto>
    {
        public RockfallReportRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async IAsyncEnumerable<HmrRockfallReport> SaveRockfallReportAsnyc(HmrSubmissionObject submission, List<RockfallReportDto> rows)
        {
            foreach (var row in rows)
            {
                row.SubmissionObjectId = submission.SubmissionObjectId;

                var entity = await DbSet
                    .Where(x => x.SubmissionObject.PartyId == submission.PartyId && x.McrrIncidentNumber == row.McrrIncidentNumber)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    submission.HmrRockfallReports
                        .Add(Mapper.Map<HmrRockfallReport>(row));
                }
                else
                {
                    row.RockfallReportId = entity.RockfallReportId;
                    Mapper.Map(row, entity);
                }

                yield return entity;
            }
        }

        public async Task<IEnumerable<RockfallReportExportDto>> ExporReportAsync(decimal submissionObjectId)
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
