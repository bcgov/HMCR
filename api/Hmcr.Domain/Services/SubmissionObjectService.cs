using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.SubmissionObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface ISubmissionObjectService
    {
        Task<SubmissionObjectDto> GetSubmissionObjectAsync(decimal submissionObjectId);
        Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string orderBy = "AppCreateTimestamp DESC", string searchText = null);
        Task<SubmissionObjectResultDto> GetSubmissionResultAsync(decimal submissionObjectId);        
    }
    public class SubmissionObjectService : ISubmissionObjectService
    {
        const string pattern = "\"rowNumber\"\\s*:\\s*(?<rowNumber>\\d*)";

        private ISubmissionObjectRepository _submissionRepo;

        public SubmissionObjectService(ISubmissionObjectRepository submissionRepo)
        {
            _submissionRepo = submissionRepo;
        }

        public async Task<SubmissionObjectDto> GetSubmissionObjectAsync(decimal submissionObjectId)
        {
            return await _submissionRepo.GetSubmissionObjectAsync(submissionObjectId);
        }

        public async Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string orderBy, string searchText)
        {
            return await _submissionRepo.GetSubmissionObjectsAsync(serviceAreaNumber, dateFrom, dateTo, pageSize, pageNumber, orderBy, searchText);
        }

        public async Task<SubmissionObjectResultDto> GetSubmissionResultAsync(decimal submissionObjectId)
        {
            var submission = await _submissionRepo.GetSubmissionResultAsync(submissionObjectId);

            //extract row number from error detail
            foreach(var row in submission.SubmissionRows)
            {
                var match = Regex.Match(row.ErrorDetail, pattern);
                row.RowNumber = match.Success ? Convert.ToInt32(match.Groups["rowNumber"].Value) : 0;
            }

            //sort by row number
            submission.SubmissionRows = submission.SubmissionRows.OrderBy(x => x.RowNumber);

            return submission;
        }
    }
}
