using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos;
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
        Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string orderBy = "AppCreateTimestamp DESC");
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

        public async Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string orderBy)
        {
            return await _submissionRepo.GetSubmissionObjectsAsync(serviceAreaNumber, dateFrom, dateTo, pageSize, pageNumber, orderBy);
        }
    }
}
