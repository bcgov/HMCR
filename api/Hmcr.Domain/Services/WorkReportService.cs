using CsvHelper;
using CsvHelper.Configuration;
using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Domain.Services.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Dtos.WorkReport;
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
    public interface IWorkReportService
    {
        Task<(Dictionary<string, List<string>> Errors, List<string> DuplicateRecordNumbers)> CheckDuplicatesAsync(FileUploadDto upload);
        Task<(decimal SubmissionObjectId, Dictionary<string, List<string>> Errors)> CreateReportAsync(FileUploadDto upload);
    }
    public class WorkReportService : ReportServiceBase, IWorkReportService
    {
        public WorkReportService(IUnitOfWork unitOfWork, 
            ISubmissionStreamService streamService, ISubmissionObjectRepository submissionRepo, ISumbissionRowRepository rowRepo, 
            IContractTermRepository contractRepo, ISubmissionStatusRepository statusRepo) 
            : base(unitOfWork, streamService, submissionRepo, rowRepo, contractRepo, statusRepo)
        {
            TableName = TableNames.WorkReport;
            CheckDuplicate = true;
            RecordNumberFieldName = "RecordNumber";
        }

        protected override async Task<bool> ParseRowsAsync(SubmissionObjectCreateDto submission, string text, Dictionary<string, List<string>> errors)
        {
            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            ConfigCsvHelper(errors, csv);
            csv.Configuration.RegisterClassMap<WorkRptInitCsvDtoMap>();

            while (csv.Read())
            {
                WorkRptInitCsvDto row = null;

                try
                {
                    row = csv.GetRecord<WorkRptInitCsvDto>();
                }
                catch (CsvHelperException)
                {
                    break;
                }

                if (row.ServiceArea != submission.ServiceAreaNumber.ToString())
                {
                    errors.AddItem("ServiceArea", $"The file contains service area which is not {submission.ServiceAreaNumber}.");
                    return false;
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();

                submission.SubmissionRows.Add(new SubmissionRowDto
                {
                    RecordNumber = row.RecordNumber,
                    RowValue = line,
                    RowHash = line.GetSha256Hash(),
                    RowStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.Row, RowStatus.Accepted),
                    EndDate = row.EndDate
                });
            }

            return errors.Count == 0;
        }
    }
}
