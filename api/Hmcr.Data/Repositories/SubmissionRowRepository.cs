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
        IAsyncEnumerable<SubmissionRowDto> FindDuplicateFromLatestRecordsAsync(decimal submissionStreamId, decimal contractTermId, IEnumerable<SubmissionRowDto> rows);
        IAsyncEnumerable<SubmissionRowDto> FindDuplicateFromAllRecordsAsync(decimal submissionStreamId, IEnumerable<SubmissionRowDto> rows);
        IAsyncEnumerable<string> UpdateIsResubmitAsync(decimal submissionStreamId, decimal contractTermId, IEnumerable<SubmissionRowDto> rows);
        Task<HmrSubmissionRow> GetSubmissionRowByRowNumAsync(decimal submissionObjectId, decimal rowNum);
        Task<HmrSubmissionRow> GetSubmissionRowByRowIdAsync(decimal rowId);
        Task<HmrSubmissionRow> GetSubmissionRowByRowIdFirstOrDefaultAsync(decimal submissionObjectId, decimal rowNum);
    }
    public class SubmissionRowRepository : HmcrRepositoryBase<HmrSubmissionRow>, ISumbissionRowRepository
    {
        private ISubmissionStatusRepository _statusRepo;

        public SubmissionRowRepository(AppDbContext dbContext, IMapper mapper, ISubmissionStatusRepository statusRepo)
            : base(dbContext, mapper)
        {
            _statusRepo = statusRepo;
        }

        public async IAsyncEnumerable<SubmissionRowDto> FindDuplicateFromLatestRecordsAsync(decimal submissionStreamId, decimal contractTermId, IEnumerable<SubmissionRowDto> rows)
        {            
            foreach (var row in rows)
            {
                var latestRow = await DbSet
                    .Where(x => x.SubmissionObject.SubmissionStreamId == submissionStreamId 
                        && x.RecordNumber == row.RecordNumber
                        && x.SubmissionObject.ContractTermId == contractTermId
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

        public async IAsyncEnumerable<string> UpdateIsResubmitAsync(decimal submissionStreamId, decimal contractTermId, IEnumerable<SubmissionRowDto> rows)
        {
            var duplicate = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.Row, RowStatus.DuplicateRow);

            foreach (var row in rows.Where(x => x.RowStatusId != duplicate))
            {
                var latestRow = await DbSet
                    .Where(x => x.SubmissionObject.SubmissionStreamId == submissionStreamId
                        && x.RecordNumber == row.RecordNumber
                        && x.SubmissionObject.ContractTermId == contractTermId
                        && x.SubmissionObject.SubmissionStatus.StatusCode == FileStatus.Success)
                    .OrderByDescending(x => x.RowId)
                    .FirstOrDefaultAsync();
                
                if (latestRow != null && latestRow.RowHash != row.RowHash)
                {
                    row.IsResubmitted = true;
                    yield return row.RecordNumber;
                }
            }
        }

        public async Task<HmrSubmissionRow> GetSubmissionRowByRowNumAsync(decimal submissionObjectId, decimal rowNum)
        {
            return await DbSet.FirstAsync(x => x.SubmissionObjectId == submissionObjectId && x.RowNum == rowNum);
        }

        public async Task<HmrSubmissionRow> GetSubmissionRowByRowIdAsync(decimal rowId)
        {
            return await DbSet.FirstAsync(x => x.RowId == rowId);
        }

        public async Task<HmrSubmissionRow> GetSubmissionRowByRowIdFirstOrDefaultAsync(decimal submissionObjectId, decimal rowNum)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.SubmissionObjectId == submissionObjectId && x.RowNum == rowNum);
        }
    }
}
