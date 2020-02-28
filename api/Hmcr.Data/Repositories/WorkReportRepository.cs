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
        IAsyncEnumerable<HmrWorkReport> SaveWorkReportAsnyc(HmrSubmissionObject submission, List<WorkReportGeometry> workReports);
        Task<IEnumerable<WorkReportExportDto>> ExporReportAsync(decimal submissionObjectId);
        Task<bool> IsActivityNumberInUseAsync(string activityNumber);
    }
    public class WorkReportRepository : HmcrRepositoryBase<HmrWorkReport>, IWorkReportRepository, IReportExportRepository<WorkReportExportDto>
    {
        public WorkReportRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async IAsyncEnumerable<HmrWorkReport> SaveWorkReportAsnyc(HmrSubmissionObject submission, List<WorkReportGeometry> workReports)
        {
            foreach (var workReport in workReports)
            {
                workReport.WorkReportTyped.SubmissionObjectId = submission.SubmissionObjectId;

                var entity = await DbSet
                    .Where(x => x.SubmissionObject.PartyId == submission.PartyId && x.RecordNumber == workReport.WorkReportTyped.RecordNumber)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    entity = Mapper.Map<HmrWorkReport>(workReport.WorkReportTyped);
                    entity.Geometry = workReport.Geometry;

                    submission.HmrWorkReports
                        .Add(entity);
                }
                else
                {
                    workReport.WorkReportTyped.WorkReportId = entity.WorkReportId;
                    Mapper.Map(workReport.WorkReportTyped, entity);
                    entity.Geometry = workReport.Geometry;
                }

                yield return entity;
            }
        }
        
        public async Task<IEnumerable<WorkReportExportDto>> ExporReportAsync(decimal submissionObjectId)
        {
            var entities = await DbSet.AsNoTracking()
                .Include(x => x.SubmissionObject)
                    .ThenInclude(x => x.Party)
                .Where(x => x.SubmissionObjectId == submissionObjectId)
                .OrderBy(x => x.RowNum)
                .ToListAsync();

            return Mapper.Map<IEnumerable<WorkReportExportDto>>(entities);
        }

        public async Task<bool> IsActivityNumberInUseAsync(string activityNumber)
        {
            return await DbSet.AnyAsync(wr => wr.ActivityNumber == activityNumber);
        }
    }
}
