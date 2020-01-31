using CsvHelper;
using CsvHelper.TypeConversion;
using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Domain.Services.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos.RockfallReport;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IRockfallReportService
    {
        Task<(Dictionary<string, List<string>> errors, List<string> resubmittedRecordNumbers)> CheckResubmitAsync(FileUploadDto upload);
        Task<(decimal submissionObjectId, Dictionary<string, List<string>> errors)> CreateReportAsync(FileUploadDto upload);
    }
    public class RockfallReportService : ReportServiceBase, IRockfallReportService
    {
        private IRockfallReportRepository _rockfallRepo;
        private ILogger<RockfallReportService> _logger;

        public RockfallReportService(IUnitOfWork unitOfWork, 
            ISubmissionStreamService streamService, ISubmissionObjectRepository submissionRepo, ISumbissionRowRepository rowRepo,
            IContractTermRepository contractRepo, ISubmissionStatusRepository statusRepo, IRockfallReportRepository rockfallRepo,
            ILogger<RockfallReportService> logger)
            : base(unitOfWork, streamService, submissionRepo, rowRepo, contractRepo, statusRepo)
        {
            TableName = TableNames.RockfallReport;
            HasRowIdentifier = true;
            RecordNumberFieldName = Fields.McrrIncidentNumber;
            DateFieldName = Fields.ReportDate;
            _rockfallRepo = rockfallRepo;
            _logger = logger;
        }

        protected override async Task<bool> ParseRowsAsync(SubmissionObjectCreateDto submission, string text, Dictionary<string, List<string>> errors)
        {
            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            CsvHelperUtils.Config(errors, csv);
            csv.Configuration.RegisterClassMap<RockfallRptInitCsvDtoMap>();

            while (csv.Read())
            {
                RockfallRptInitCsvDto row = null;

                try
                {
                    row = csv.GetRecord<RockfallRptInitCsvDto>();
                }
                catch (TypeConverterException ex)
                {
                    errors.AddItem(ex.MemberMapData.Member.Name, ex.Message);
                    break;
                }
                catch (CsvHelperException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    throw;
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();

                submission.SubmissionRows.Add(new SubmissionRowDto
                {
                    RecordNumber = row.McrrIncidentNumber,
                    RowValue = line,
                    RowHash = line.GetSha256Hash(),
                    RowStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.Row, RowStatus.RowReceived),
                    EndDate = (DateTime)row.ReportDate,
                    RowNum = csv.Context.Row - 1
                });
            }

            return errors.Count == 0;
        }
    }
}
