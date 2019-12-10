using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.SubmissionObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface ISubmissionObjectService
    {
        Task<SubmissionObjectDto> GetSubmissionObjectAsync(decimal submissionObjectId);
        Task<IEnumerable<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo);
    }
    public class SubmissionObjectService : ISubmissionObjectService
    {
        private ISubmissionObjectRepository _submissionRepo;

        public SubmissionObjectService(ISubmissionObjectRepository submissionRepo)
        {
            _submissionRepo = submissionRepo;
        }

        public async Task<SubmissionObjectDto> GetSubmissionObjectAsync(decimal submissionObjectId)
        {
            return await _submissionRepo.GetSubmissionObjectAsync(submissionObjectId);
        }

        public async Task<IEnumerable<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo)
        {
            return await _submissionRepo.GetSubmissionObjectsAsync(serviceAreaNumber, dateFrom, dateTo);
        }
    }
}
