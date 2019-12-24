using CsvHelper;
using CsvHelper.Configuration;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
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
        Task StartBackgroundProcess(decimal submissionObjectId);
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

        public async Task StartBackgroundProcess(decimal submissionObjectId)
        {
            //todo: check if there's any file submitted for the service area to be processed 

            var errors = new Dictionary<string, List<string>>();

            var submission = await _submissionRepo.GetSubmissionObjectEntityAsync(submissionObjectId); //get the staged rows too
            var accepted = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.File, RowStatus.Accepted);
            var duplicate = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.Row, RowStatus.Duplicate);

            Console.WriteLine($"Hangfire job {submissionObjectId}");

            if (submission.SubmissionStatusId != accepted)
            {
                return;
            }

            var rows = ParseRowsUnTyped(submission, errors);

            SetRowIdAndRemoveDuplicate(submission, duplicate, rows);

            //todo: check if a staged row exists

            //validate and populate staged row

        }

        private static void SetRowIdAndRemoveDuplicate(HmrSubmissionObject submission, decimal duplicate, List<WorkRptUntypedCsvDto> rows)
        {
            for (int i = rows.Count - 1; i <= 0; i--)
            {
                var row = rows[i];
                var entity = submission.HmrSubmissionRows.First(x => x.RecordNumber == row.RecordNumber);

                if (entity.RowStatusId == duplicate)
                {
                    rows.RemoveAt(i);
                    continue;
                }

                row.RowId = entity.RowId;
            }
        }

        private List<WorkRptUntypedCsvDto> ParseRowsUnTyped(HmrSubmissionObject submission, Dictionary<string, List<string>> errors)
        {
            var text = Encoding.UTF8.GetString(submission.DigitalRepresentation);

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            ConfigCsvHelper(errors, csv, false);
            csv.Configuration.RegisterClassMap<WorkRptUntypedCsvDtoMap>();

            return csv.GetRecords<WorkRptUntypedCsvDto>().ToList();
        }
    }
}
