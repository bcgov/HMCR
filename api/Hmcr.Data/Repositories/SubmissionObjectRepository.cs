using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface ISubmissionObjectRepository
    {
        Task<HmrSubmissionObject> CreateSubmissionObjectAsync(SubmissionObjectCreateDto submission);
        Task<SubmissionObjectDto> GetSubmissionObjectAsync(decimal submissionObjectId);
        Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string searchText, string orderBy, string direction);
        Task<bool> IsDuplicateFileAsync(SubmissionObjectCreateDto submission);
        Task<HmrSubmissionObject> GetSubmissionObjectEntityAsync(decimal submissionObjectId);
        SubmissionDto[] GetSubmissionObjecsForBackgroundJob(decimal serviceAreaNumber);
        Task<SubmissionObjectResultDto> GetSubmissionResultAsync(decimal submissionObjectId);
        Task<SubmissionObjectFileDto> GetSubmissionFileAsync(decimal submissionObjectId);
        Task<HmrSubmissionObject> GetSubmissionObjecForBackgroundJobAsync(decimal submissionObjectId);
        Task<SubmissionDto> GetSubmissionInfoForExportAsync(decimal submissionObjectId);
        Task<SubmissionInfoForEmailDto> GetSubmissionInfoForEmailAsync(decimal submissionObjectId);
        Task<bool> UpdateSubmissionStatusAsync(decimal submissionObjectId, decimal submissionStatusId, long concurrencyControlNumber);
    }
    public class SubmissionObjectRepository : HmcrRepositoryBase<HmrSubmissionObject>, ISubmissionObjectRepository
    {
        public SubmissionObjectRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<HmrSubmissionObject> CreateSubmissionObjectAsync(SubmissionObjectCreateDto submission)
        {
            var submissionEntity = await AddAsync(submission);

            foreach (var row in submission.SubmissionRows)
            {
                submissionEntity.HmrSubmissionRows
                    .Add(Mapper.Map<HmrSubmissionRow>(row));
            }

            return submissionEntity;
        }

        public async Task<SubmissionObjectDto> GetSubmissionObjectAsync(decimal submissionObjectId)
        {
            return await GetByIdAsync<SubmissionObjectDto>(submissionObjectId);
        }

        public async Task<HmrSubmissionObject> GetSubmissionObjectEntityAsync(decimal submissionObjectId)
        {
            return await DbSet.Include(x => x.HmrSubmissionRows).FirstAsync(x => x.SubmissionObjectId == submissionObjectId);
        }

        public SubmissionDto[] GetSubmissionObjecsForBackgroundJob(decimal serviceAreaNumber)
        {
            var submissions = DbSet.AsNoTracking()
                .Where(x => x.ServiceAreaNumber == serviceAreaNumber && x.DigitalRepresentation != null && x.SubmissionStatus.StatusType == StatusType.File &&
                    (x.SubmissionStatus.StatusCode == FileStatus.FileReceived || x.SubmissionStatus.StatusCode == FileStatus.FileInProgress))
                .Include(x => x.MimeType)
                .Include(x => x.SubmissionStream)
                .OrderBy(x => x.SubmissionObjectId) //must be ascending order
                .ToArray();

            return Mapper.Map<SubmissionDto[]>(submissions);
        }

        public async Task<SubmissionDto> GetSubmissionInfoForExportAsync(decimal submissionObjectId)
        {
            var submission = await DbSet.AsNoTracking()
                .Include(x => x.MimeType)
                .Include(x => x.SubmissionStream)
                .FirstOrDefaultAsync(x => x.SubmissionObjectId == submissionObjectId);

            return Mapper.Map<SubmissionDto>(submission);
        }

        public async Task<HmrSubmissionObject> GetSubmissionObjecForBackgroundJobAsync(decimal submissionObjectId)
        {
            return await DbSet.FirstAsync(x => x.SubmissionObjectId == submissionObjectId);
        }

        public async Task<SubmissionObjectResultDto> GetSubmissionResultAsync(decimal submissionObjectId)
        {
            var submission = await DbSet.AsNoTracking()
                .Select(x => new SubmissionObjectResultDto
                {
                    SubmissionObjectId = x.SubmissionObjectId,
                    FileName = x.FileName,
                    SubmissionStreamId = x.SubmissionStreamId,
                    StreamName = x.SubmissionStream.StreamName,
                    ServiceAreaNumber = x.ServiceAreaNumber,
                    SubmissionStatusId = x.SubmissionStatusId,
                    SubmissionStatusCode = x.SubmissionStatus.StatusCode,
                    Description = x.SubmissionStatus.Description,
                    ErrorDetail = x.ErrorDetail,
                    AppCreateTimestamp = x.AppCreateTimestamp,
                    SubmissionRows = x.HmrSubmissionRows.Where(y => y.ErrorDetail != null || y.WarningDetail != null).Select(y => new SubmissionRowResultDto
                    {
                        RowId = y.RowId,
                        SubmissionObjectId = y.SubmissionObjectId,
                        RowStatusId = y.RowStatusId,
                        Description = y.RowStatus.Description,
                        RecordNumber = y.RecordNumber,
                        RowValue = y.RowValue,
                        ErrorDetail = y.ErrorDetail,
                        WarningDetail = y.WarningDetail,
                        StartVariance = y.StartVariance,
                        EndVariance = y.EndVariance,
                        RowNum = y.RowNum,
                        IsResubmitted = y.IsResubmitted ?? false
                    }).OrderBy(y => y.RowNum)
                })
                .FirstOrDefaultAsync(x => x.SubmissionObjectId == submissionObjectId);

            return submission;
        }

        public async Task<SubmissionObjectFileDto> GetSubmissionFileAsync(decimal submissionObjectId)
        {
            var submissionEntity = await DbSet.AsNoTracking()
                .Include(x => x.MimeType)
                .FirstOrDefaultAsync(x => x.SubmissionObjectId == submissionObjectId);

            var submission = Mapper.Map<SubmissionObjectFileDto>(submissionEntity);

            return submission;
        }

        public async Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string searchText, string orderBy, string direction)
        {
            var query = DbSet.AsNoTracking()
                .Where(x => x.ServiceAreaNumber == serviceAreaNumber && x.AppCreateTimestamp >= dateFrom && x.AppCreateTimestamp <= dateTo)
                .Join(DbContext.HmrSystemUsers,
                    x => x.AppCreateUserGuid,
                    y => y.UserGuid,
                    (x, y) => new SubmissionObjectSearchDto
                    {
                        SubmissionObjectId = x.SubmissionObjectId,
                        AppCreateTimestamp = x.AppCreateTimestamp,
                        AppCreateUserid = x.AppCreateUserid,
                        FileName = x.FileName,
                        FirstName = y.FirstName,
                        LastName = y.LastName,
                        SubmissionStreamId = x.SubmissionStreamId,
                        StreamName = x.SubmissionStream.StreamName,
                        ServiceAreaNumber = x.ServiceAreaNumber,
                        SubmissionStatusId = x.SubmissionStatusId,
                        SubmissionStatusCode = x.SubmissionStatus.StatusCode,
                        Description = x.SubmissionStatus.Description
                    });

            if (searchText.IsNotEmpty())
            {
                searchText = searchText.Trim();

                query = query
                    .Where(u => u.FirstName.Contains(searchText) || u.LastName.Contains(searchText) || (u.FirstName + " " + u.LastName).Contains(searchText));
            }

            return await Page<SubmissionObjectSearchDto, SubmissionObjectSearchDto>(query, pageSize, pageNumber, orderBy, direction);
        }

        public async Task<SubmissionInfoForEmailDto> GetSubmissionInfoForEmailAsync(decimal submissionObjectId)
        {
            var query = await DbSet.AsNoTracking()
                 .Select(x => new SubmissionInfoForEmailDto
                 {
                     SubmissionObjectId = (long)x.SubmissionObjectId,
                     FileName = x.FileName,
                     FileType = x.SubmissionStream.StreamName,
                     SubmissionDate = x.AppCreateTimestamp,
                     ServiceAreaNumber = (int)x.ServiceAreaNumber,
                     NumOfRecords = x.HmrSubmissionRows.Count(),
                     NumOfErrorRecords = x.HmrSubmissionRows.Count(y => y.ErrorDetail != null),
                     Success = x.ErrorDetail == null,
                     NumOfDuplicateRecords = x.HmrSubmissionRows.Count(y => y.RowStatus.StatusCode == RowStatus.RowDuplicate),
                     NumOfReplacedRecords = x.HmrSubmissionRows.Count(y => y.IsResubmitted == true),
                     NumOfWarningRecords = x.HmrSubmissionRows.Count(y => y.WarningDetail != null),
                     SubmissionStatus = x.SubmissionStatus.StatusCode,
                     ErrorDetail = x.ErrorDetail,
                 })
                 .FirstAsync(x => x.SubmissionObjectId == submissionObjectId);

            return query;
        }

        public async Task<bool> IsDuplicateFileAsync(SubmissionObjectCreateDto submission)
        {
            //check only the previous submission regardless of success or failure in order to cover the following scenarios

            //S1.
            //  1. user submits a file #1 (success)
            //  2. user submits a file #2 (success)
            //  3. user submits a file #1 again 
            //  The net result should be file #1 submission success instead of duplicate file error.

            //S2.
            //  1. user submits a file #1 (fail while processing )
            //  2. user submits a file #2 (success)
            //  3. user submits a file #1 again 
            //  The net result should be file #1 submission error instead of duplicate file error.

            var latestFile = await DbSet
                .Include(x => x.SubmissionStatus)
                .Where(x => x.SubmissionStreamId == submission.SubmissionStreamId &&
                    x.ServiceAreaNumber == submission.ServiceAreaNumber)
                .OrderByDescending(x => x.SubmissionObjectId)
                .FirstOrDefaultAsync();

            if (latestFile == null)
                return false;

            //if it was internal error, the user should be able to upload again.
            if (latestFile.SubmissionStatus.StatusCode == FileStatus.FileUnexpectedError)
                return false;

            return latestFile.FileHash == submission.FileHash;
        }

        public async Task<bool> UpdateSubmissionStatusAsync(decimal submissionObjectId, decimal submissionStatusId, long concurrencyControlNumber)
        {
            var sql = new StringBuilder("UPDATE HMR_SUBMISSION_OBJECT SET ");
            sql.Append("SUBMISSION_STATUS_ID = {0}, ");
            sql.Append("CONCURRENCY_CONTROL_NUMBER = CONCURRENCY_CONTROL_NUMBER + 1 ");
            sql.Append("WHERE SUBMISSION_OBJECT_ID = {1} AND CONCURRENCY_CONTROL_NUMBER = {2} ");

            var rowcount = await DbContext.Database.ExecuteSqlRawAsync(sql.ToString(), submissionStatusId, submissionObjectId, concurrencyControlNumber);

            return rowcount == 1;
        }
    }
}
