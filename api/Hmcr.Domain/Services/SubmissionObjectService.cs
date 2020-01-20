﻿using Hmcr.Data.Repositories;
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
        Task<SubmissionObjectFileDto> GetSubmissionFileAsync(decimal submissionObjectId);
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

        public async Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string orderBy, string searchText)
        {
            return await _submissionRepo.GetSubmissionObjectsAsync(serviceAreaNumber, dateFrom, dateTo, pageSize, pageNumber, orderBy, searchText);
        }

        public async Task<SubmissionObjectResultDto> GetSubmissionResultAsync(decimal submissionObjectId)
        {
            return await _submissionRepo.GetSubmissionResultAsync(submissionObjectId);
        }

        public async Task<SubmissionObjectFileDto> GetSubmissionFileAsync(decimal submissionObjectId)
        {
            return await _submissionRepo.GetSubmissionFileAsync(submissionObjectId);
        }
    }
}
