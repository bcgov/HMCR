using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.SubmissionStatus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface ISubmissionStatusService
    {
        Task<IEnumerable<SubmissionStatusDto>> GetSubmissionStatusAsync();
    }
    public class SubmissionStatusService : ISubmissionStatusService
    {
        private ISubmissionStatusRepository _statusRepo;

        public SubmissionStatusService(ISubmissionStatusRepository statusRepo)
        {
            _statusRepo = statusRepo;
        }

        public async Task<IEnumerable<SubmissionStatusDto>> GetSubmissionStatusAsync()
        {
            return await _statusRepo.GetActiveStatuses();
        }
    }
}
