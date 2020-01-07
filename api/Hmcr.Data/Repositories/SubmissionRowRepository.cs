using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionRow;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Hmcr.Data.Repositories
{
    public interface ISumbissionRowRepository
    {
        IAsyncEnumerable<SubmissionRowDto> FindDuplicateRowsAsync(decimal submissionStreamId, decimal partyId, IEnumerable<SubmissionRowDto> rows);
        IAsyncEnumerable<string> FindDuplicateRowsToOverwriteAsync(decimal submissionStreamId, decimal partyId, IEnumerable<SubmissionRowDto> rows);
    }
    public class SubmissionRowRepository : HmcrRepositoryBase<HmrSubmissionRow>, ISumbissionRowRepository
    {
        private ISubmissionStatusRepository _statusRepo;

        public SubmissionRowRepository(AppDbContext dbContext, IMapper mapper, ISubmissionStatusRepository statusRepo)
            : base(dbContext, mapper)
        {
            _statusRepo = statusRepo;
        }

        public async IAsyncEnumerable<SubmissionRowDto> FindDuplicateRowsAsync(decimal submissionStreamId, decimal partyId, IEnumerable<SubmissionRowDto> rows)
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

        public async IAsyncEnumerable<string> FindDuplicateRowsToOverwriteAsync(decimal submissionStreamId, decimal partyId, IEnumerable<SubmissionRowDto> rows)
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
                    yield return row.RecordNumber;
            }
        }
    }
}
