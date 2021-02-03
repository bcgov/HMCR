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
        Task<(bool itemExists, List<string> conflicts)> IsReportedWorkReportForLocationAAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports);
        Task<(bool itemExists, List<string> conflicts)> IsReportedWorkReportForLocationBAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports);
        Task<(bool itemExists, List<string> conflicts)> IsReportedWorkReportForLocationCAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports);
        
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

        public async Task<(bool itemExists, List<string> conflicts)> IsReportedWorkReportForLocationAAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports)
        {
            List<string> conflicts = new List<string>();

            int daysBetween = (int)workReportTyped.ActivityCodeValidation.ReportingFrequency;
            DateTime endDate = workReportTyped.EndDate.GetValueOrDefault(DateTime.Today).Date;
            DateTime futureEndDate = endDate.AddDays(daysBetween);
            DateTime startDate = endDate.AddDays(-daysBetween);

            var foundInDB = await DbContext.HmrWorkReportVws.AsNoTracking()
                .Where(x => x.ActivityNumber == workReportTyped.ActivityNumber
                        && x.ServiceArea == workReportTyped.ServiceArea
                        && ((x.EndDate > startDate && x.EndDate <= endDate)
                        || (x.EndDate >= endDate && x.EndDate < futureEndDate))
                        && x.ValidationStatus.ToUpper().StartsWith(RowStatus.RowSuccess)
                ).ToListAsync();
            
            foreach (var foundItem in foundInDB)
            {
                conflicts.Add(foundItem.RecordNumber);
            }

            var foundInReport = workReports
                .Where(x => x.WorkReportTyped.ActivityNumber == workReportTyped.ActivityNumber
                        && x.WorkReportTyped.RecordNumber != workReportTyped.RecordNumber
                        && x.WorkReportTyped.ServiceArea == workReportTyped.ServiceArea
                        && ((x.WorkReportTyped.EndDate > startDate && x.WorkReportTyped.EndDate <= endDate)
                        || (x.WorkReportTyped.EndDate >= endDate && x.WorkReportTyped.EndDate < futureEndDate))
                 ).ToList();

            foreach (var foundItem in foundInReport)
            {
                conflicts.Add(foundItem.WorkReportTyped.RecordNumber);
            }

            return (conflicts.Count() > 0, conflicts);
        }

        public async Task<(bool itemExists, List<string> conflicts)> IsReportedWorkReportForLocationBAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports)
        {
            List<string> conflicts = new List<string>();
            
            int daysBetween = (int)workReportTyped.ActivityCodeValidation.ReportingFrequency;
            DateTime endDate = workReportTyped.EndDate.GetValueOrDefault(DateTime.Today).Date;
            DateTime futureEndDate = endDate.AddDays(daysBetween);
            DateTime startDate = endDate.AddDays(-daysBetween);

            var foundInDB = await DbContext.HmrWorkReportVws.AsNoTracking()
                .Where(x => x.ActivityNumber == workReportTyped.ActivityNumber
                        && x.RecordNumber != workReportTyped.RecordNumber
                        && x.ServiceArea == workReportTyped.ServiceArea
                        && x.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && ((x.EndDate > startDate && x.EndDate <= endDate)
                        || (x.EndDate >= endDate && x.EndDate < futureEndDate))
                        && x.ValidationStatus.ToUpper().StartsWith(RowStatus.RowSuccess)
                ).ToListAsync();

            foreach (var foundItem in foundInDB)
            {
                conflicts.Add(foundItem.RecordNumber);
            }

            var foundInReport = workReports
                .Where(x => x.WorkReportTyped.ActivityNumber == workReportTyped.ActivityNumber
                        && x.WorkReportTyped.RecordNumber != workReportTyped.RecordNumber
                        && x.WorkReportTyped.ServiceArea == workReportTyped.ServiceArea
                        && x.WorkReportTyped.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && ((x.WorkReportTyped.EndDate > startDate && x.WorkReportTyped.EndDate <= endDate)
                        || (x.WorkReportTyped.EndDate >= endDate && x.WorkReportTyped.EndDate < futureEndDate))
                 ).ToList();

            foreach (var foundItem in foundInReport)
            {
                conflicts.Add(foundItem.WorkReportTyped.RecordNumber);
            }

            return (conflicts.Count() > 0, conflicts);
        }
        
        public async Task<(bool itemExists, List<string> conflicts)> IsReportedWorkReportForLocationCAsync(WorkReportTyped workReportTyped, List<WorkReportGeometry> workReports)
        {
            List<string> conflicts = new List<string>();

            decimal md = 0.1M; //100m
            decimal startOffset = 0.0M;
            decimal endOffset = 0.0M;
            decimal foundStartOffset = 0.0M;
            decimal foundEndOffset = 0.0M;

            int daysBetween = (int)workReportTyped.ActivityCodeValidation.ReportingFrequency;
            DateTime endDate = workReportTyped.EndDate.GetValueOrDefault(DateTime.Today).Date;
            DateTime futureEndDate = endDate.AddDays(daysBetween);
            DateTime startDate = endDate.AddDays(-daysBetween);

            if (workReportTyped.FeatureType == FeatureType.Point)
            {
                startOffset = (decimal)workReportTyped.StartOffset - md;
                endOffset = (decimal)workReportTyped.StartOffset + md;
            } else if (workReportTyped.FeatureType == FeatureType.Line)
            {
                if (workReportTyped.StartOffset > workReportTyped.EndOffset)
                {
                    //the line is reversed we will flip them to handle this
                    startOffset = (decimal)workReportTyped.EndOffset - md;
                    endOffset = (decimal)workReportTyped.StartOffset + md;
                    /*startOffset = (decimal)workReportTyped.StartOffset + md;
                    endOffset = (decimal)workReportTyped.EndOffset - md;*/
                }
                else
                {
                    startOffset = (decimal)workReportTyped.StartOffset - md;
                    endOffset = (decimal)workReportTyped.EndOffset + md;
                }
            }

            //find items in the database that match based on standard criteria
            var foundInDB = await DbContext.HmrWorkReportVws.AsNoTracking()
                .Where(x => x.ActivityNumber == workReportTyped.ActivityNumber
                        && x.RecordNumber != workReportTyped.RecordNumber
                        && x.ServiceArea == workReportTyped.ServiceArea
                        && x.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && ((x.EndDate > startDate && x.EndDate <= endDate)
                        || (x.EndDate >= endDate && x.EndDate < futureEndDate))
                        && x.SiteNumber == workReportTyped.SiteNumber
                        && x.StructureNumber == workReportTyped.StructureNumber
                        && x.ValidationStatus.ToUpper().StartsWith(RowStatus.RowSuccess)
                ).ToListAsync();

            foreach (var foundItem in foundInDB)
            {
                (foundStartOffset, foundEndOffset) = normalizeOffsets(foundItem.StartOffset, foundItem.EndOffset);

                //check that the offsets don't overlap?
                if (!((endOffset <= foundStartOffset) || (foundEndOffset <= startOffset)))
                {
                    conflicts.Add(foundItem.RecordNumber);
                }

                //reset the found start/end offsets
                foundStartOffset = 0.0M;
                foundEndOffset = 0.0M;
            }

            var foundInReport = workReports
                .Where(x => x.WorkReportTyped.ActivityNumber == workReportTyped.ActivityNumber
                        && x.WorkReportTyped.RecordNumber != workReportTyped.RecordNumber
                        && x.WorkReportTyped.ServiceArea == workReportTyped.ServiceArea
                        && x.WorkReportTyped.HighwayUnique.ToLower() == workReportTyped.HighwayUnique.ToLower()
                        && ((x.WorkReportTyped.EndDate > startDate && x.WorkReportTyped.EndDate <= endDate)
                        || (x.WorkReportTyped.EndDate >= endDate && x.WorkReportTyped.EndDate < futureEndDate))
                        && x.WorkReportTyped.SiteNumber == workReportTyped.SiteNumber
                        && x.WorkReportTyped.StructureNumber == workReportTyped.StructureNumber
                 ).ToList();

            foreach (var foundItem in foundInReport)
            {
                (foundStartOffset, foundEndOffset) = normalizeOffsets(foundItem.WorkReportTyped.StartOffset, foundItem.WorkReportTyped.EndOffset);

                //check that the line/points don't overlap?
                if (!((endOffset <= foundStartOffset) || (foundEndOffset <= startOffset)))
                {
                    conflicts.Add(foundItem.WorkReportTyped.RecordNumber);
                }
                
                //reset them
                foundStartOffset = 0.0M;
                foundEndOffset = 0.0M;
            }

            //there is a scenario where a row is being overwritten so it exists in the database
            // and the work report, if it's a conflict it'll show up twice (db and report). 
            // we need to remove the duplicates to handle this.

            conflicts = conflicts.Distinct().ToList();
            conflicts.Sort();
            return (conflicts.Count() > 0, conflicts);
        }

        private (decimal startOffset, decimal endOffset) normalizeOffsets(decimal? startOffset, decimal? endOffset)
        {
            //perform the offset matching
            var foundStartOffset = (decimal)startOffset;
            var foundEndOffset = (decimal)((endOffset == null)
                ? startOffset
                : endOffset);

            //sometimes the offsets can be flipped, we need to adjust them for matching
            if (foundStartOffset > foundEndOffset)
            {
                foundStartOffset = foundEndOffset;
                foundEndOffset = (decimal)startOffset;
            }

            return (foundStartOffset, foundEndOffset);
        }
    }
}