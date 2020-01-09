using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.RockfallReport;
using Hmcr.Model.Dtos.WorkReport;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Hmcr.Data.Repositories
{
    public interface IRockfallReportRepository
    {
        IAsyncEnumerable<HmrRockfallReport> SaveRockfallReportAsnyc(HmrSubmissionObject submission, List<RockfallReportDto> rows);
    }
    public class RockfallReportRepository : HmcrRepositoryBase<HmrRockfallReport>, IRockfallReportRepository
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
                    .Where(x => x.SubmissionObject.PartyId == submission.PartyId && x.MajorIncidentNumber == row.MajorIncidentNumber)
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
    }
}
