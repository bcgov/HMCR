using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.RockfallReport;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.WildlifeReport;
using Hmcr.Model.Dtos.WorkReport;
using Hmcr.Model.Utils;
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
        Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string searchText, string orderBy, string direction);
        Task<SubmissionObjectResultDto> GetSubmissionResultAsync(decimal submissionObjectId);
        Task<SubmissionObjectFileDto> GetSubmissionFileAsync(decimal submissionObjectId);
        Task<(SubmissionDto submission, byte[] file)> ExportSubmissionCsvAsync(decimal submissionObjectId);
    }
    public class SubmissionObjectService : ISubmissionObjectService
    {
        private ISubmissionObjectRepository _submissionRepo;
        private IWorkReportRepository _workRptRepo;
        private IRockfallReportRepository _rockfallRptRepo;
        private IWildlifeReportRepository _wildlifeRptRepo;

        public SubmissionObjectService(ISubmissionObjectRepository submissionRepo,
            IWorkReportRepository workRptRepo, IRockfallReportRepository rockfallRptRepo, IWildlifeReportRepository wildlifeRptRepo)
        {
            _submissionRepo = submissionRepo;
            _workRptRepo = workRptRepo;
            _rockfallRptRepo = rockfallRptRepo;
            _wildlifeRptRepo = wildlifeRptRepo;
        }

        public async Task<SubmissionObjectDto> GetSubmissionObjectAsync(decimal submissionObjectId)
        {
            return await _submissionRepo.GetSubmissionObjectAsync(submissionObjectId);
        }

        public async Task<PagedDto<SubmissionObjectSearchDto>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, int pageSize, int pageNumber, string searchText, string orderBy, string direction)
        {
            return await _submissionRepo.GetSubmissionObjectsAsync(serviceAreaNumber, dateFrom, dateTo, pageSize, pageNumber, searchText, orderBy, direction);
        }

        public async Task<SubmissionObjectResultDto> GetSubmissionResultAsync(decimal submissionObjectId)
        {
            return await _submissionRepo.GetSubmissionResultAsync(submissionObjectId);
        }

        public async Task<SubmissionObjectFileDto> GetSubmissionFileAsync(decimal submissionObjectId)
        {
            return await _submissionRepo.GetSubmissionFileAsync(submissionObjectId);
        }

        public async Task<(SubmissionDto submission, byte[] file)> ExportSubmissionCsvAsync(decimal submissionObjectId)
        {
            var submission = await _submissionRepo.GetSubmissionInfoForExportAsync(submissionObjectId);

            if (submission == null)
                return (null, null);

            switch (submission.StagingTableName)
            {
                case TableNames.WorkReport:
                    return (submission, await ExportToCsvAsync(submissionObjectId, (IReportExportRepository<WorkReportExportDto>)_workRptRepo));
                case TableNames.RockfallReport:
                    return (submission, await ExportToCsvAsync(submissionObjectId, (IReportExportRepository<RockfallReportExportDto>)_rockfallRptRepo));
                case TableNames.WildlifeReport:
                    return (submission, await ExportToCsvAsync(submissionObjectId, (IReportExportRepository<WildlifeReportExportDto>)_wildlifeRptRepo));
                default:
                    throw new NotImplementedException($"Background job for {submission.StagingTableName} is not implemented.");
            }
        }

        private async Task<byte[]> ExportToCsvAsync<T>(decimal submissionObjectId, IReportExportRepository<T> repo) where T : IReportExportDto
        {
            var report = await repo.ExportReportAsync(submissionObjectId);

            if (report.Count() == 0)
            {
                return null;
            }

            var rptCsv = string.Join(Environment.NewLine, report.Select(x => x.ToCsv()));
            rptCsv = $"{CsvUtils.GetCsvHeader<T>()}{Environment.NewLine}{rptCsv}";

            var encoding = new UTF8Encoding();
            return encoding.GetBytes(rptCsv);
        }
    }
}
