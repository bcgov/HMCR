using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.WildlifeReport;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Hmcr.Data.Repositories
{
    public interface IWildlifeReportRepository
    {
        void SaveWildlifeReportAsnyc(HmrSubmissionObject submission, List<WildlifeReportDto> rows);
    }
    public class WildlifeReportRepository : HmcrRepositoryBase<HmrWildlifeReport>, IWildlifeReportRepository
    {
        public WildlifeReportRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public void SaveWildlifeReportAsnyc(HmrSubmissionObject submission, List<WildlifeReportDto> rows)
        {
            foreach (var row in rows)
            {
                row.SubmissionObjectId = submission.SubmissionObjectId;

                submission.HmrWildlifeReports
                    .Add(Mapper.Map<HmrWildlifeReport>(row));
            }
        }
    }
}
