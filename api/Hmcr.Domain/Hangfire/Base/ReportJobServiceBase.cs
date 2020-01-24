using CsvHelper;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.ActivityCode;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.WorkReport;
using Hmcr.Model.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Hangfire.Base
{
    public class ReportJobServiceBase
    {
        protected IUnitOfWork _unitOfWork;
        protected ISubmissionStatusRepository _statusRepo;
        protected ISubmissionObjectRepository _submissionRepo;
        protected ISumbissionRowRepository _submissionRowRepo;

        protected decimal _duplicateRowStatusId;
        protected decimal _errorRowStatusId;
        protected decimal _successRowStatusId;
        protected decimal _errorFileStatusId;
        protected decimal _successFileStatusId;
        protected decimal _inProgressRowStatusId;

        protected HmrSubmissionObject _submission;

        public ReportJobServiceBase(IUnitOfWork unitOfWork,
            ISubmissionStatusRepository statusRepo, ISubmissionObjectRepository submissionRepo,
            ISumbissionRowRepository submissionRowRepo)
        {
            _unitOfWork = unitOfWork;
            _statusRepo = statusRepo;
            _submissionRepo = submissionRepo;
            _submissionRowRepo = submissionRowRepo;
        }

        protected async Task SetStatusesAsync()
        {
            var statuses = await _statusRepo.GetActiveStatuses();

            _duplicateRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.DuplicateRow).StatusId;
            _errorRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.RowError).StatusId;
            _successRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.Success).StatusId;

            _errorFileStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.DataError).StatusId;
            _successFileStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.Success).StatusId;

            _inProgressRowStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.InProgress).StatusId;
        }

        protected async Task SetSubmissionAsync(SubmissionDto submissionDto)
        {
            _submission = await _submissionRepo.GetSubmissionObjecForBackgroundJobAsync(submissionDto.SubmissionObjectId);
            _submission.SubmissionStatusId = _inProgressRowStatusId;
            _unitOfWork.Commit();
        }

        protected bool CheckCommonMandatoryHeaders<T1, T2>(List<T1> rows, T2 headers, Dictionary<string, List<string>> errors) where T2 : IReportHeaders
        {
            if (rows.Count == 0) //not possible since it's already validated in the ReportServiceBase.
                throw new Exception("File has no rows.");

            var row = rows[0];

            var fields = typeof(T1).GetProperties();

            foreach (var field in fields)
            {
                if (!headers.CommonMandatoryFields.Any(x => x == field.Name))
                    continue;

                if (field.GetValue(row) == null)
                {
                    errors.AddItem("File", $"Header [{field.Name.WordToWords()}] is missing");
                }
            }

            return errors.Count == 0;
        }

        protected async Task<string> SetRowIdAndRemoveDuplicate<T>(List<T> untypedRows, string headers) where T : IReportCsvDto
        {
            headers = $"{Fields.RowNum}," + headers;
            var text = new StringBuilder();
            text.AppendLine(headers);

            for (int i = untypedRows.Count - 1; i >= 0; i--)
            {
                var untypedRow = untypedRows[i];
                var entity = await _submissionRowRepo.GetSubmissionRowByRowNum(_submission.SubmissionObjectId, (decimal)untypedRow.RowNum);

                if (entity.RowStatusId == _duplicateRowStatusId)
                {
                    untypedRows.RemoveAt(i);
                    continue;
                }

                text.AppendLine($"{untypedRow.RowNum},{entity.RowValue}");
                untypedRow.RowId = entity.RowId;
            }

            return text.ToString();
        }

        protected void SetErrorDetail(HmrSubmissionRow submissionRow, Dictionary<string, List<string>> errors)
        {
            submissionRow.RowStatusId = _errorRowStatusId;
            submissionRow.ErrorDetail = errors.GetErrorDetail();
            _submission.ErrorDetail = FileError.ReferToRowErrors;
            _submission.SubmissionStatusId = _errorFileStatusId;
        }
        protected string GetHeader(string text)
        {
            if (text == null)
                return "";

            using var reader = new StringReader(text);
            var header = reader.ReadLine().Replace("\"", "");

            return header ?? "";
        }
    }
}
