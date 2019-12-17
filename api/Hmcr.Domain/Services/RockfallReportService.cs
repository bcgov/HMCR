using CsvHelper;
using CsvHelper.Configuration;
using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Domain.Services.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos.RockfallReport;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IRockfallReportService
    {
        Task<(Dictionary<string, List<string>> Errors, List<string> DuplicateRecordNumbers)> CheckDuplicatesAsync(FileUploadDto upload);
        Task<(decimal SubmissionObjectId, Dictionary<string, List<string>> Errors)> CreateReportAsync(FileUploadDto upload);

    }
    public class RockfallReportService : ReportServiceBase, IRockfallReportService
    {

        public RockfallReportService(IUnitOfWork unitOfWork, 
            ISubmissionStreamService streamService, ISubmissionObjectRepository submissionRepo, ISumbissionRowRepository rowRepo,
            IContractTermRepository contractRepo, ISubmissionStatusRepository statusRepo)
            : base(unitOfWork, streamService, submissionRepo, rowRepo, contractRepo, statusRepo)
        {
            TableName = TableNames.RockfallReport;
            CheckDuplicate = true;
            RecordNumberFieldName = "MajorIncidentNumber";
        }

        protected override async Task<bool> ParseRowsAsync(SubmissionObjectCreateDto submission, string text, Dictionary<string, List<string>> errors)
        {
            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            ConfigCsvHelper(errors, csv);
            csv.Configuration.RegisterClassMap<RockfallRptInitCsvDtoMap>();

            while (csv.Read())
            {
                RockfallRptInitCsvDto row = null;

                try
                {
                    row = csv.GetRecord<RockfallRptInitCsvDto>();
                }
                catch (CsvHelperException)
                {
                    break;
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();

                submission.SubmissionRows.Add(new SubmissionRowDto
                {
                    RecordNumber = row.MajorIncidentNumber,
                    RowValue = line,
                    RowHash = line.GetSha256Hash(),
                    RowStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.Row, RowStatus.Accepted),
                    EndDate = (DateTime)row.ReportDate
                });
            }

            return errors.Count == 0;
        }

    }
}
