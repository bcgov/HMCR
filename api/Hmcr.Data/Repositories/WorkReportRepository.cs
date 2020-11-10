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
        Task<bool> IsReportedWorkReportForLocationAAsync(WorkReportTyped workReportTyped);
        Task<bool> IsReportedWorkReportForLocationBAsync(WorkReportTyped workReportTyped);
        Task<bool> IsReportedWorkReportForLocationCPointAsync(WorkReportTyped workReportTyped);
        Task<bool> IsReportedWorkReportForLocationCLineAsync(WorkReportTyped workReportTyped);
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

        public async Task<bool> IsReportedWorkReportForLocationAAsync(WorkReportTyped workReportTyped)
        {
            if (workReportTyped.ActivityCodeValidation.ReportingFrequency == null
                || workReportTyped.ActivityCodeValidation.ReportingFrequency < 1) return false;
            DateTime eDate = workReportTyped.EndDate.GetValueOrDefault(DateTime.Today).Date;
            int days = (int)workReportTyped.ActivityCodeValidation.ReportingFrequency;
            DateTime sDate = eDate.AddDays(-days);
            return await DbContext.HmrWorkReportVws.AsNoTracking()
                .Where(x => x.ActivityNumber == workReportTyped.ActivityNumber
                        && x.ServiceArea == workReportTyped.ServiceArea
                        && (x.EndDate > sDate && x.EndDate <= eDate)
                        && x.ValidationStatus.ToUpper().StartsWith(RowStatus.RowSuccess)
                )
                .AnyAsync();
        }
        public async Task<bool> IsReportedWorkReportForLocationBAsync(WorkReportTyped workReportTyped)
        {
            if (workReportTyped.ActivityCodeValidation.ReportingFrequency == null
                || workReportTyped.ActivityCodeValidation.ReportingFrequency < 1) return false;
            DateTime eDate = workReportTyped.EndDate.GetValueOrDefault(DateTime.Today).Date;
            int days = (int)workReportTyped.ActivityCodeValidation.ReportingFrequency;
            DateTime sDate = eDate.AddDays(-days);
            return await DbContext.HmrWorkReportVws.AsNoTracking()
                .Where(x => x.ActivityNumber == workReportTyped.ActivityNumber
                        && x.ServiceArea == workReportTyped.ServiceArea
                        && x.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && (x.EndDate > sDate && x.EndDate <= eDate)
                        && x.ValidationStatus.ToUpper().StartsWith(RowStatus.RowSuccess)
                )
                .AnyAsync();
        }
        public async Task<bool> IsReportedWorkReportForLocationCPointAsync(WorkReportTyped workReportTyped)
        {
            decimal md = (decimal)0.1; //100m
            decimal startOffset = (decimal)workReportTyped.StartOffset - md;
            decimal endOffset = (decimal)workReportTyped.StartOffset + md;
            if (workReportTyped.ActivityCodeValidation.ReportingFrequency == null
                || workReportTyped.ActivityCodeValidation.ReportingFrequency < 1) return false;
            DateTime eDate = workReportTyped.EndDate.GetValueOrDefault(DateTime.Today).Date;
            int days = (int)workReportTyped.ActivityCodeValidation.ReportingFrequency;
            DateTime sDate = eDate.AddDays(-days);
            return await DbContext.HmrWorkReportVws.AsNoTracking()
                .Where(x => x.ActivityNumber == workReportTyped.ActivityNumber
                        && x.ServiceArea == workReportTyped.ServiceArea
                        && x.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && (x.EndDate > sDate && x.EndDate <= eDate)
                        && (x.StartOffset != null && x.EndOffset == null)
                        && (x.StartOffset> startOffset && x.StartOffset< endOffset)
                        && x.ValidationStatus.ToUpper().StartsWith(RowStatus.RowSuccess)
                )
                .AnyAsync();
        }
        public async Task<bool> IsReportedWorkReportForLocationCLineAsync(WorkReportTyped workReportTyped)
        {
            decimal md = (decimal)0.1; //100m
            decimal startOffset = (decimal)workReportTyped.StartOffset - md;
            decimal endOffset = (decimal)workReportTyped.EndOffset + md;
            if (workReportTyped.ActivityCodeValidation.ReportingFrequency == null
                || workReportTyped.ActivityCodeValidation.ReportingFrequency < 1) return false;
            DateTime eDate = workReportTyped.EndDate.GetValueOrDefault(DateTime.Today).Date;
            int days = (int)workReportTyped.ActivityCodeValidation.ReportingFrequency;
            DateTime sDate = eDate.AddDays(-days);
            return await DbContext.HmrWorkReportVws.AsNoTracking()
                .Where(x => x.ActivityNumber == workReportTyped.ActivityNumber
                        && x.ServiceArea == workReportTyped.ServiceArea
                        && x.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && (x.EndDate > sDate && x.EndDate <= eDate)
                        && (x.StartOffset != null && x.EndOffset != null)
                        && (x.StartOffset > startOffset || x.EndOffset < endOffset)
                        && x.ValidationStatus.ToUpper().StartsWith(RowStatus.RowSuccess)
                )
                .AnyAsync();
        }
    }
}
