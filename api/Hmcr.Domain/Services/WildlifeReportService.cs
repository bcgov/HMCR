using CsvHelper;
using CsvHelper.TypeConversion;
using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Domain.Services.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Dtos.WildlifeReport;
using Hmcr.Model.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IWildlifeReportService
    {
        Task<(decimal submissionObjectId, Dictionary<string, List<string>> errors)> CreateReportAsync(FileUploadDto upload);
    }
    public class WildlifeReportService : ReportServiceBase, IWildlifeReportService
    {
        private IWildlifeReportRepository _wildlifeRepo;
        private ILogger<WildlifeReportService> _logger;

        public WildlifeReportService(IUnitOfWork unitOfWork,
            ISubmissionStreamService streamService, ISubmissionObjectRepository submissionRepo, ISumbissionRowRepository rowRepo,
            IContractTermRepository contractRepo, ISubmissionStatusRepository statusRepo, IWildlifeReportRepository wildlifeRepo, IFieldValidatorService validator,
            ILogger<WildlifeReportService> logger)
            : base(unitOfWork, streamService, submissionRepo, rowRepo, contractRepo, statusRepo, validator)
        {
            TableName = TableNames.WildlifeReport;
            HasRowIdentifier = false;
            DateFieldName = "Today";
            _wildlifeRepo = wildlifeRepo;
            _logger = logger;
        }
        protected override async Task<bool> ParseRowsAsync(SubmissionObjectCreateDto submission, string text, Dictionary<string, List<string>> errors)
        {
            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            CsvHelperUtils.Config(errors, csv);
            csv.Configuration.RegisterClassMap<WildlifeRptInitCsvDtoMap>();

            var serviceArea = (long)submission.ServiceAreaNumber;

            var rows = new List<WildlifeRptInitCsvDto>();
            while (csv.Read())
            {
                WildlifeRptInitCsvDto row = null;

                try
                {
                    row = csv.GetRecord<WildlifeRptInitCsvDto>();
                    rows.Add(row);
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

                if (row.ServiceArea != serviceArea.ToString())
                {
                    errors.AddItem("ServiceArea", $"The file contains service area which is not {submission.ServiceAreaNumber}.");
                    return false;
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();

                submission.SubmissionRows.Add(new SubmissionRowDto
                {
                    RecordNumber = null,
                    RowValue = line,
                    RowHash = line.GetSha256Hash(),
                    RowStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.Row, RowStatus.RowReceived),
                    EndDate = row.AccidentDate ?? Constants.MinDate,
                    RowNum = csv.Context.Row - 1
                });
            }

            if (errors.Count == 0)
            {
                Validate(rows, Entities.WildlifeReportInit, errors);
            }

            return errors.Count == 0;
        }
    }
}
