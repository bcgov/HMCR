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
        Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string orderBy = "AppCreateTimestamp DESC", string searchText = null);
        Task<bool> IsDuplicateFileAsync(SubmissionObjectCreateDto submission);
        Task<HmrSubmissionObject> GetSubmissionObjectEntityAsync(decimal submissionObjectId);
        Task<SubmissionDto[]> GetSubmissionObjecsForBackgroundJobAsync(decimal serviceAreaNumber);
        Task<SubmissionObjectResultDto> GetSubmissionResultAsync(decimal submissionObjectId);
        Task<SubmissionObjectFileDto> GetSubmissionFileAsync(decimal submissionObjectId);
        Task<HmrSubmissionObject> GetSubmissionObjecForBackgroundJobAsync(decimal submissionObjectId);
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

            foreach(var row in submission.SubmissionRows)
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

        public async Task<SubmissionDto[]> GetSubmissionObjecsForBackgroundJobAsync(decimal serviceAreaNumber)
        {
            var acceptedStatus = await DbContext.HmrSubmissionStatus.FirstAsync(x => (x.StatusCode == FileStatus.FileReceived || x.StatusCode == FileStatus.InProgress) && x.StatusType == StatusType.File);

            var submissions = await DbSet.AsNoTracking()
                .Where(x => x.ServiceAreaNumber == serviceAreaNumber && x.SubmissionStatus.StatusType == StatusType.File &&
                    (x.SubmissionStatus.StatusCode == FileStatus.FileReceived || x.SubmissionStatus.StatusCode == FileStatus.InProgress))
                .Select(x => new SubmissionDto
                {
                    SubmissionObjectId = x.SubmissionObjectId,
                    StagingTableName = x.SubmissionStream.StagingTableName
                })
                .OrderBy(x => x.SubmissionObjectId) //must be ascending order
                .ToArrayAsync();                

            return submissions;
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
                    SubmissionRows = x.HmrSubmissionRows.Where(y => y.ErrorDetail != null).Select(y => new SubmissionRowResultDto
                    {
                        RowId = y.RowId,
                        SubmissionObjectId = y.SubmissionObjectId,
                        RowStatusId = y.RowStatusId,
                        Description = y.RowStatus.Description,
                        RecordNumber = y.RecordNumber,
                        RowValue = y.RowValue,
                        ErrorDetail = y.ErrorDetail,
                        RowNum = y.RowNum,
                        IsResubmitted = y.IsResubmitted
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

        public async Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string orderBy, string searchText)
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
                query = query
                    .Where(u => u.FirstName.Contains(searchText) || u.LastName.Contains(searchText));
            }

            return await Page<SubmissionObjectSearchDto, SubmissionObjectSearchDto>(query, pageSize, pageNumber, orderBy);
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
                .Where(x => x.SubmissionStreamId == submission.SubmissionStreamId && x.ServiceAreaNumber == submission.ServiceAreaNumber) 
                .OrderByDescending(x => x.SubmissionObjectId)
                .FirstOrDefaultAsync();

            if (latestFile == null)
                return false;

            return latestFile.FileHash == submission.FileHash;
        }
    }
}
