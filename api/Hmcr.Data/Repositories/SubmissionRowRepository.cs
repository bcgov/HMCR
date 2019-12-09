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
        IAsyncEnumerable<SubmissionRowDto> FindDuplicateRowsAsync(IEnumerable<SubmissionRowDto> rows);
        IAsyncEnumerable<int> FindDuplicateRowsAsync(IEnumerable<string> rows);
        IAsyncEnumerable<string> FindDuplicateRowsToOverwriteAsync(IEnumerable<SubmissionRowDto> rows);
    }
    public class SubmissionRowRepository : HmcrRepositoryBase<HmrSubmissionRow>, ISumbissionRowRepository
    {
        private ISubmissionStatusRepository _statusRepo;

        public SubmissionRowRepository(AppDbContext dbContext, IMapper mapper, ISubmissionStatusRepository statusRepo)
            : base(dbContext, mapper)
        {
            _statusRepo = statusRepo;
        }

        public async IAsyncEnumerable<SubmissionRowDto> FindDuplicateRowsAsync(IEnumerable<SubmissionRowDto> rows)
        {
            foreach (var row in rows)
            {
                if (await DbSet.AnyAsync(x => x.RowHash == row.RowHash))
                    yield return row;
            }
        }

        public async IAsyncEnumerable<int> FindDuplicateRowsAsync(IEnumerable<string> rows)
        {
            var i = 1;
            foreach (var row in rows)
            {
                if (await DbSet.AnyAsync(x => x.RowHash == row.GetSha256Hash()))
                    yield return i;

                i++;
            }
        }

        public async IAsyncEnumerable<string> FindDuplicateRowsToOverwriteAsync(IEnumerable<SubmissionRowDto> rows)
        {
            var duplicate = await _statusRepo.GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.Duplicate);

            foreach (var row in rows.Where(x => x.RowStatusId != duplicate))
            {
                var query = await DbSet
                    .Include(x => x.SubmissionObject)
                        .ThenInclude(x => x.ServiceAreaNumberNavigation)
                            .ThenInclude(x => x.HmrContractTerms)
                    .Where(x => x.RecordNumber == row.RecordNumber && x.RowStatusId != duplicate)
                    .SelectMany(x => x.SubmissionObject.ServiceAreaNumberNavigation.HmrContractTerms)
                    .AnyAsync(x => x.StartDate <= row.EndDate && x.EndDate > row.EndDate);
                    
                if (query)
                    yield return row.RecordNumber;
            }
        }
    }
}
