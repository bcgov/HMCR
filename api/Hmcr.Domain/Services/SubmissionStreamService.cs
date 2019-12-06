using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.SubmissionStream;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface ISubmissionStreamService
    {
        Task<IEnumerable<SubmissionStreamDto>> GetSubmissionStreamsAsync(bool? isActive = true);
        Task<SubmissionStreamDto> GetSubmissionStreamByTableNameAsync(string tableName);
    }
    public class SubmissionStreamService : ISubmissionStreamService
    {
        private ISubmissionStreamRepository _streamRepo;

        public SubmissionStreamService(ISubmissionStreamRepository streamRepo)
        {
            _streamRepo = streamRepo;
        }

        public async Task<IEnumerable<SubmissionStreamDto>> GetSubmissionStreamsAsync(bool? isActive = true)
        {
            return await _streamRepo.GetSubmissionStreamsAsync(isActive);
        }

        public async Task<SubmissionStreamDto> GetSubmissionStreamByTableNameAsync(string tableName)
        {
            return await _streamRepo.GetSubmissionStreamByTableNameAsync(tableName);
        }
    }
}
