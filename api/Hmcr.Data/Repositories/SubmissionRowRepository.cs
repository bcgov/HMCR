using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model;
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
    public interface ISumbissionRowRepository
    {
        IAsyncEnumerable<SubmissionRowDto> FindDuplicateRowsAsync(decimal submissionStreamId, IEnumerable<SubmissionRowDto> rows);
        IAsyncEnumerable<int> FindDuplicateRowsAsync(decimal submissionStreamId, IEnumerable<string> rows);
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

        public async IAsyncEnumerable<SubmissionRowDto> FindDuplicateRowsAsync(decimal submissionStreamId, IEnumerable<SubmissionRowDto> rows)
        {            
            foreach (var row in rows)
            {
                var query = DbSet
                    .AnyAsync(x => x.SubmissionObject.SubmissionStreamId == submissionStreamId && x.RowHash == row.RowHash);

                if (await query)
                    yield return row;
            }
        }

        public async IAsyncEnumerable<int> FindDuplicateRowsAsync(decimal submissionStreamId, IEnumerable<string> rows)
        {
            var i = 1;
            foreach (var row in rows)
            {
                var query = DbSet
                    .AnyAsync(x => x.SubmissionObject.SubmissionStreamId == submissionStreamId && x.RowHash == row.GetSha256Hash());

                if (await query)
                    yield return i;

                i++;
            }
        }

        public async IAsyncEnumerable<string> FindDuplicateRowsToOverwriteAsync(decimal submissionStreamId, decimal partyId, IEnumerable<SubmissionRowDto> rows)
        {
            var duplicate = await _statusRepo.GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.Duplicate);

            foreach (var row in rows.Where(x => x.RowStatusId != duplicate))
            {
                var query = await DbSet
                    .Where(x => x.SubmissionObject.SubmissionStreamId == submissionStreamId && x.RecordNumber == row.RecordNumber && x.RowStatusId != duplicate)
                    .SelectMany(x => x.SubmissionObject.ServiceAreaNumberNavigation.HmrContractTerms)
                    .AnyAsync(x => x.PartyId == partyId);
                    
                if (query)
                    yield return row.RecordNumber;
            }
        }
    }
}
