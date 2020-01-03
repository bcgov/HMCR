using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.SubmissionObject;
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
        Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string orderBy = "AppCreateTimestamp DESC");
        Task<bool> IsDuplicateFileAsync(SubmissionObjectCreateDto submission);
        Task<HmrSubmissionObject> GetSubmissionObjectEntityAsync(decimal submissionObjectId);
        Task<HmrSubmissionObject[]> GetSubmissionObjecsForBackgroundJobAsync(decimal serviceAreaNumber);
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

        public async Task<HmrSubmissionObject[]> GetSubmissionObjecsForBackgroundJobAsync(decimal serviceAreaNumber)
        {
            var acceptedStatus = await DbContext.HmrSubmissionStatus.FirstAsync(x => x.StatusCode == FileStatus.Accepted && x.StatusType == StatusType.File);

            var submissions = await DbSet
                .Where(x => x.ServiceAreaNumber == serviceAreaNumber && x.SubmissionStatusId == acceptedStatus.StatusId)
                .Include(x => x.SubmissionStream)
                .Include(x => x.SubmissionStatus)
                .Include(x => x.HmrSubmissionRows)
                .OrderBy(x => x.SubmissionObjectId)
                .ToArrayAsync();

            return submissions;
        }

        //public async Task<IEnumerable<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo)
        //{
        //    var submissions = await DbSet.AsNoTracking()
        //        .Include(x => x.SubmissionStatus)
        //        .Include(x => x.SubmissionStream)
        //        .Where(x => x.ServiceAreaNumber == serviceAreaNumber && x.AppCreateTimestamp >= dateFrom && x.AppCreateTimestamp <= dateTo)
        //        .Join(DbContext.HmrSystemUsers, 
        //            x => x.AppCreateUserGuid, 
        //            y => y.UserGuid, 
        //            (x, y) => new SubmissionObjectSearchDto
        //            {
        //                SubmissionObjectId = x.SubmissionObjectId,
        //                AppCreateTimestamp = x.AppCreateTimestamp,
        //                AppCreateUserid = x.AppCreateUserid,
        //                FileName = x.FileName,
        //                FirstName = y.FirstName,
        //                LastName = y.LastName,
        //                SubmissionStreamId = x.SubmissionStreamId,
        //                StreamName = x.SubmissionStream.StreamName,
        //                ServiceAreaNumber = x.ServiceAreaNumber,
        //                SubmissionStatusId = x.SubmissionStatusId,
        //                SubmissionStatusCode = x.SubmissionStatus.StatusCode,
        //                Description = x.SubmissionStatus.Description
        //            })
        //        .OrderBy(x => x.AppCreateTimestamp)
        //        .ToListAsync();

        //    return submissions;
        //}

        public async Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string orderBy)
        {
            var query = DbSet.AsNoTracking()
                .Include(x => x.SubmissionStatus)
                .Include(x => x.SubmissionStream)
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
