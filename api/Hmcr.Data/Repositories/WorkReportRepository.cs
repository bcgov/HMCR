using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.WorkReport;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IWorkReportRepository
    {
        IAsyncEnumerable<HmrWorkReport> SaveWorkReportAsnyc(HmrSubmissionObject submission, List<WorkReportDto> rows);
    }
    public class WorkReportRepository : HmcrRepositoryBase<HmrWorkReport>, IWorkReportRepository
    {
        public WorkReportRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async IAsyncEnumerable<HmrWorkReport> SaveWorkReportAsnyc(HmrSubmissionObject submission, List<WorkReportDto> rows)
        {
            foreach (var row in rows)
            {
                row.SubmissionObjectId = submission.SubmissionObjectId;

                var entity = await DbSet
                    .Where(x => x.SubmissionObject.PartyId == submission.PartyId && x.RecordNumber == row.RecordNumber)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    submission.HmrWorkReports
                        .Add(Mapper.Map<HmrWorkReport>(row));
                }
                else
                {
                    row.WorkReportId = entity.WorkReportId;
                    Mapper.Map(row, entity);
                }

                yield return entity;
            }
        }
    }
}
