using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;

namespace Hmcr.Data.Repositories
{
    public interface IWorkReportRepository
    {        
    }
    public class WorkReportRepository : HmcrRepositoryBase<HmrWorkReport>, IWorkReportRepository
    {
        private ISubmissionStatusRepository _statusRepo;

        public WorkReportRepository(AppDbContext dbContext, IMapper mapper, ISubmissionStatusRepository statusRepo)
            : base(dbContext, mapper)
        {
            _statusRepo = statusRepo;
        }
    }
}
