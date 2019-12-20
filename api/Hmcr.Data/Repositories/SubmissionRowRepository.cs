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
    }
    public class SubmissionRowRepository : HmcrRepositoryBase<HmrSubmissionRow>, ISumbissionRowRepository
    {
        public SubmissionRowRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async IAsyncEnumerable<SubmissionRowDto> FindDuplicateRowsAsync(IEnumerable<SubmissionRowDto> rows)
        {
            foreach (var row in rows)
            {
                if (await DbSet.AnyAsync(x => x.RowHash == row.RowHash))
                    yield return row;
            }
        }

        public async IAsyncEnumerable<string> FindDuplicateRowsToOverrideAsync(IEnumerable<SubmissionRowDto> rows)
        {
            foreach (var row in rows.Where(x => x.RowStatusId != RowStatus.Duplicate))
            {
                if (await DbSet.AnyAsync(x => x.RecordNumber == row.RecordNumber)) //todo need to join with contract term
                    yield return row.RecordNumber;
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
    }
}
