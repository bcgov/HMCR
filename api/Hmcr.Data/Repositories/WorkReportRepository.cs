using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos.WorkReport;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IWorkReportRepository
    {
        IAsyncEnumerable<HmrWorkReport> SaveWorkReportAsnyc(HmrSubmissionObject submission, List<WorkReportGeometry> workReports);
        Task<IEnumerable<WorkReportExportDto>> ExportReportAsync(decimal submissionObjectId);
        Task<bool> IsActivityNumberInUseAsync(string activityNumber);
        Task<bool> IsReportedWorkReportForLocationAAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports);
        Task<bool> IsReportedWorkReportForLocationBAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports);
        Task<bool> IsReportedWorkReportForLocationCPointAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports);
        Task<bool> IsReportedWorkReportForLocationCLineAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports);
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
                    int currentVersioinNumber = entity.RecordVersionNumber;
                    Mapper.Map(workReport.WorkReportTyped, entity);
                    entity.Geometry = workReport.Geometry;
                    entity.RecordVersionNumber = currentVersioinNumber + 1;
                }
                yield return entity;
            }
        }
        
        public async Task<IEnumerable<WorkReportExportDto>> ExportReportAsync(decimal submissionObjectId)
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

        public async Task<bool> IsReportedWorkReportForLocationAAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports)
        {
            if (workReportTyped.ActivityCodeValidation.ReportingFrequency == null
                || workReportTyped.ActivityCodeValidation.ReportingFrequency < 1) return false;

            int daysBetween = (int)workReportTyped.ActivityCodeValidation.ReportingFrequency;
            DateTime endDate = workReportTyped.EndDate.GetValueOrDefault(DateTime.Today).Date;
            DateTime futureEndDate = endDate.AddDays(daysBetween);
            DateTime startDate = endDate.AddDays(-daysBetween);

            var existsInDB = await DbContext.HmrWorkReportVws.AsNoTracking()
                .Where(x => x.ActivityNumber == workReportTyped.ActivityNumber
                        && x.ServiceArea == workReportTyped.ServiceArea
                        && ((x.EndDate > startDate && x.EndDate <= endDate)
                        || (x.EndDate > endDate && x.EndDate <= futureEndDate))
                        && x.ValidationStatus.ToUpper().StartsWith(RowStatus.RowSuccess)
                )
                .AnyAsync();

            var existsInReport = workReports.Where(x => x.WorkReportTyped.ActivityNumber == workReportTyped.ActivityNumber
                        && x.WorkReportTyped.RecordNumber != workReportTyped.RecordNumber
                        && x.WorkReportTyped.ServiceArea == workReportTyped.ServiceArea
                        && ((x.WorkReportTyped.EndDate > startDate && x.WorkReportTyped.EndDate <= endDate)
                        || (x.WorkReportTyped.EndDate > endDate && x.WorkReportTyped.EndDate <= futureEndDate))
                        ).Any();

            return (existsInDB || existsInReport);
        }

        public async Task<bool> IsReportedWorkReportForLocationBAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports)
        {
            if (workReportTyped.ActivityCodeValidation.ReportingFrequency == null
                || workReportTyped.ActivityCodeValidation.ReportingFrequency < 1) return false;

            int daysBetween = (int)workReportTyped.ActivityCodeValidation.ReportingFrequency;
            DateTime endDate = workReportTyped.EndDate.GetValueOrDefault(DateTime.Today).Date;
            DateTime futureEndDate = endDate.AddDays(daysBetween);
            DateTime startDate = endDate.AddDays(-daysBetween);

            var existsInDB = await DbContext.HmrWorkReportVws.AsNoTracking()
                .Where(x => x.ActivityNumber == workReportTyped.ActivityNumber
                        && x.ServiceArea == workReportTyped.ServiceArea
                        && x.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && ((x.EndDate > startDate && x.EndDate <= endDate)
                        || (x.EndDate > endDate && x.EndDate <= futureEndDate))
                        && x.ValidationStatus.ToUpper().StartsWith(RowStatus.RowSuccess)
                )
                .AnyAsync();

            var existsInReport = workReports.Where(x => x.WorkReportTyped.ActivityNumber == workReportTyped.ActivityNumber
                        && x.WorkReportTyped.RecordNumber != workReportTyped.RecordNumber
                        && x.WorkReportTyped.ServiceArea == workReportTyped.ServiceArea
                        && x.WorkReportTyped.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && ((x.WorkReportTyped.EndDate > startDate && x.WorkReportTyped.EndDate <= endDate)
                        || (x.WorkReportTyped.EndDate > endDate && x.WorkReportTyped.EndDate <= futureEndDate))
                        ).Any();

            return (existsInDB || existsInReport);
        }

        public async Task<bool> IsReportedWorkReportForLocationCPointAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports)
        {
            decimal md = 0.1M; //100m
            decimal startOffset = (decimal)workReportTyped.StartOffset - md;
            decimal endOffset = (decimal)workReportTyped.StartOffset + md;

            if (workReportTyped.ActivityCodeValidation.ReportingFrequency == null
                || workReportTyped.ActivityCodeValidation.ReportingFrequency < 1) return false;

            int daysBetween = (int)workReportTyped.ActivityCodeValidation.ReportingFrequency;
            DateTime endDate = workReportTyped.EndDate.GetValueOrDefault(DateTime.Today).Date;
            DateTime futureEndDate = endDate.AddDays(daysBetween);
            DateTime startDate = endDate.AddDays(-daysBetween);

            var existsInDB = await DbContext.HmrWorkReportVws.AsNoTracking()
                .Where(x => x.ActivityNumber == workReportTyped.ActivityNumber
                        && x.ServiceArea == workReportTyped.ServiceArea
                        && x.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && ((x.EndDate > startDate && x.EndDate <= endDate)
                        || (x.EndDate > endDate && x.EndDate <= futureEndDate))
                        && x.StartOffset != null && x.EndOffset == null
                        && ((x.StartOffset >= startOffset) && (x.StartOffset <= endOffset)
                        || (x.EndOffset >= endOffset) && (x.EndOffset <= startOffset))
                        && x.ValidationStatus.ToUpper().StartsWith(RowStatus.RowSuccess)
                )
                .AnyAsync();
            
            var existsInReport = workReports.Where(x => x.WorkReportTyped.ActivityNumber == workReportTyped.ActivityNumber
                        && x.WorkReportTyped.RecordNumber != workReportTyped.RecordNumber
                        && x.WorkReportTyped.ServiceArea == workReportTyped.ServiceArea
                        && x.WorkReportTyped.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && ((x.WorkReportTyped.EndDate > startDate && x.WorkReportTyped.EndDate <= endDate)
                        || (x.WorkReportTyped.EndDate > endDate && x.WorkReportTyped.EndDate <= futureEndDate))
                        && x.WorkReportTyped.StartOffset != null && x.WorkReportTyped.EndOffset == null
                        && ((x.WorkReportTyped.StartOffset >= startOffset) && (x.WorkReportTyped.StartOffset <= endOffset)
                        || (x.WorkReportTyped.EndOffset >= endOffset) && (x.WorkReportTyped.EndOffset <= startOffset))
                        ).Any();

            return (existsInDB || existsInReport);
        }

        public async Task<bool> IsReportedWorkReportForLocationCLineAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports)
        {
            decimal md = 0.1M; //100m
            decimal startOffset = 0.0M;
            decimal endOffset = 0.0M;
            
            if (workReportTyped.StartOffset > workReportTyped.EndOffset)
            {
                startOffset= (decimal)workReportTyped.StartOffset + md;
                endOffset = (decimal)workReportTyped.EndOffset - md;
            } else
            {
                startOffset = (decimal)workReportTyped.StartOffset - md;
                endOffset = (decimal)workReportTyped.EndOffset + md;
            }
            
            if (workReportTyped.ActivityCodeValidation.ReportingFrequency == null
                || workReportTyped.ActivityCodeValidation.ReportingFrequency < 1) return false;

            int daysBetween = (int)workReportTyped.ActivityCodeValidation.ReportingFrequency;
            DateTime endDate = workReportTyped.EndDate.GetValueOrDefault(DateTime.Today).Date;
            DateTime futureEndDate = endDate.AddDays(daysBetween);
            DateTime startDate = endDate.AddDays(-daysBetween);

            var existsInDB = await DbContext.HmrWorkReportVws.AsNoTracking()
                .Where(x => x.ActivityNumber == workReportTyped.ActivityNumber
                        && x.ServiceArea == workReportTyped.ServiceArea
                        && x.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && ((x.EndDate > startDate && x.EndDate <= endDate)
                        || (x.EndDate > endDate && x.EndDate <= futureEndDate))
                        && (x.StartOffset != null && x.EndOffset != null)
                        && ((x.StartOffset >= startOffset) && (x.StartOffset <= endOffset)
                        || (x.EndOffset >= endOffset) && (x.EndOffset <= startOffset))
                        && x.ValidationStatus.ToUpper().StartsWith(RowStatus.RowSuccess)
                )
                .AnyAsync();
            
            var existsInReport = workReports.Where(x => x.WorkReportTyped.ActivityNumber == workReportTyped.ActivityNumber
                        && x.WorkReportTyped.RecordNumber != workReportTyped.RecordNumber
                        && x.WorkReportTyped.ServiceArea == workReportTyped.ServiceArea
                        && x.WorkReportTyped.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && ((x.WorkReportTyped.EndDate > startDate && x.WorkReportTyped.EndDate <= endDate)
                        || (x.WorkReportTyped.EndDate > endDate && x.WorkReportTyped.EndDate <= futureEndDate))
                        && x.WorkReportTyped.StartOffset != null && x.WorkReportTyped.EndOffset != null
                        && ((x.WorkReportTyped.StartOffset >= startOffset) && (x.WorkReportTyped.StartOffset <= endOffset)
                        || (x.WorkReportTyped.EndOffset >= endOffset) && (x.WorkReportTyped.EndOffset <= startOffset))
                        ).Any();

            return (existsInDB || existsInReport);
        }
    }
}
