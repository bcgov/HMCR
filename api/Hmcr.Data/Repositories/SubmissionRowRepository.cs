using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionRow;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface ISumbissionRowRepository
    {
        IAsyncEnumerable<SubmissionRowDto> FindDuplicateFromLatestRecordsAsync(decimal submissionStreamId, decimal partyId, IEnumerable<SubmissionRowDto> rows);
        IAsyncEnumerable<SubmissionRowDto> FindDuplicateFromAllRecordsAsync(decimal submissionStreamId, IEnumerable<SubmissionRowDto> rows);
        IAsyncEnumerable<string> UpdateIsResubmitAsync(decimal submissionStreamId, decimal partyId, IEnumerable<SubmissionRowDto> rows);
        Task<HmrSubmissionRow> GetSubmissionRowByRowNum(decimal submissionObjectId, decimal rowNum);
        Task<HmrSubmissionRow> GetSubmissionRowByRowId(decimal rowId);
    }
    public class SubmissionRowRepository : HmcrRepositoryBase<HmrSubmissionRow>, ISumbissionRowRepository
    {
        private ISubmissionStatusRepository _statusRepo;

        public SubmissionRowRepository(AppDbContext dbContext, IMapper mapper, ISubmissionStatusRepository statusRepo)
            : base(dbContext, mapper)
        {
            _statusRepo = statusRepo;
        }

        public async IAsyncEnumerable<SubmissionRowDto> FindDuplicateFromLatestRecordsAsync(decimal submissionStreamId, decimal partyId, IEnumerable<SubmissionRowDto> rows)
        {            
            foreach (var row in rows)
            {
                var latestRow = await DbSet
                    .Where(x => x.SubmissionObject.SubmissionStreamId == submissionStreamId 
                        && x.RecordNumber == row.RecordNumber
                        && x.SubmissionObject.PartyId == partyId
                        && x.SubmissionObject.SubmissionStatus.StatusCode == FileStatus.Success)
                    .OrderByDescending(x => x.RowId)
                    .FirstOrDefaultAsync();

                if (latestRow != null && latestRow.RowHash == row.RowHash)
                    yield return row;
            }
        }

        public async IAsyncEnumerable<SubmissionRowDto> FindDuplicateFromAllRecordsAsync(decimal submissionStreamId, IEnumerable<SubmissionRowDto> rows)
        {
            foreach (var row in rows)
            {
                var duplicate = await DbSet
                    .Where(x => x.SubmissionObject.SubmissionStreamId == submissionStreamId
                        && x.RowHash == row.RowHash
                        && x.SubmissionObject.SubmissionStatus.StatusCode == FileStatus.Success)
                    .FirstOrDefaultAsync();

                if (duplicate != null)
                    yield return row;
            }
        }

        public async IAsyncEnumerable<string> UpdateIsResubmitAsync(decimal submissionStreamId, decimal partyId, IEnumerable<SubmissionRowDto> rows)
        {
            var duplicate = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.Row, RowStatus.DuplicateRow);

            foreach (var row in rows.Where(x => x.RowStatusId != duplicate))
            {
                var latestRow = await DbSet
                    .Where(x => x.SubmissionObject.SubmissionStreamId == submissionStreamId
                        && x.RecordNumber == row.RecordNumber
                        && x.SubmissionObject.PartyId == partyId
                        && x.SubmissionObject.SubmissionStatus.StatusCode == FileStatus.Success)
                    .OrderByDescending(x => x.RowId)
                    .FirstOrDefaultAsync();
                
                if (latestRow != null && latestRow.RowHash != row.RowHash)
                {
                    row.IsResubmitted = "Y";
                    yield return row.RecordNumber;
                }
            }
        }

        public async Task<HmrSubmissionRow> GetSubmissionRowByRowNum(decimal submissionObjectId, decimal rowNum)
        {
            return await DbSet.FirstAsync(x => x.SubmissionObjectId == submissionObjectId && x.RowNum == rowNum);
        }

        public async Task<HmrSubmissionRow> GetSubmissionRowByRowId(decimal rowId)
        {
            return await DbSet.FirstAsync(x => x.RowId == rowId);
        }
    }
}
