using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
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
            return await DbSet
                .AnyAsync(x => x.SubmissionStreamId == submission.SubmissionStreamId && x.FileHash == submission.FileHash);
        }
    }
}
