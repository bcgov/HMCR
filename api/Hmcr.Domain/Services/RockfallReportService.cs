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
using System.Globalization;
using System.IO;
using System.Linq;
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
            IContractTermRepository contractRepo, ISubmissionStatusService statusService, IRockfallReportRepository rockfallRepo, IFieldValidatorService validator,
            ILogger<RockfallReportService> logger, IServiceAreaService saService)
            : base(unitOfWork, streamService, submissionRepo, rowRepo, contractRepo, statusService, validator, saService)
        {
            TableName = TableNames.RockfallReport;
            HasRowIdentifier = true;
            RecordNumberFieldName = Fields.McrrIncidentNumber;
            DateFieldName = Fields.ReportDate;
            _rockfallRepo = rockfallRepo;
            _logger = logger;
        }

        protected override async Task<bool> ParseRowsAsync(SubmissionObjectCreateDto submission, TextReader textReader, Dictionary<string, List<string>> errors)
        {
            using var csv = new CsvReader(textReader, CultureInfo.InvariantCulture);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<RockfallRptInitCsvDtoMap>();

            var serviceAreastrings = ConvertServiceAreaToStrings(submission.ServiceAreaNumber);
            var serviceArea = await _saService.GetServiceAreaByServiceAreaNumberAsyc(submission.ServiceAreaNumber);

            var headerValidated = false;
            var rows = new List<RockfallRptInitCsvDto>();
            var rowNum = 1;

            while (csv.Read())
            {
                RockfallRptInitCsvDto row = null;

                try
                {
                    row = csv.GetRecord<RockfallRptInitCsvDto>();

                    if (!headerValidated)
                    {
                        if (!CheckCommonMandatoryFields(csv.Context.HeaderRecord, RockfallReportHeaders.MandatoryFields, errors))
                        {
                            return false;
                        }
                        else
                        {
                            headerValidated = true;
                        }
                    }

                    row.RowNum = ++rowNum;
                    row.ServiceArea = serviceArea.ConvertToServiceAreaString(row.ServiceArea);
                    rows.Add(row);
                }
                catch (TypeConverterException ex)
                {
                    errors.AddItem(ex.MemberMapData.Member.Name, ex.Message);
                    break;
                }
                catch (CsvHelper.MissingFieldException)
                {
                    break; //handled in CsvHelperUtils
                }
                catch (CsvHelper.ReaderException ex)
                {
                    _logger.LogWarning(ex.Message);
                    errors.AddItem("Report Type", "Please make sure the report type selected is correct.");
                    return false;
                }
                catch (CsvHelperException ex)
                {
                    _logger.LogInformation(ex.ToString());
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    throw;
                }

                if (!serviceAreastrings.Contains(row.ServiceArea))
                {
                    errors.AddItem(Fields.ServiceArea, $"The file contains service area which is not {serviceAreastrings[0]}.");
                    return false;
                }

                if (row.McrrIncidentNumber.IsEmpty())
                {
                    errors.AddItem(Fields.McrrIncidentNumber, $"MCRR Incident Number is missing for row [{csv.Context.Row}].");
                    return false;
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();

                submission.SubmissionRows.Add(new SubmissionRowDto
                {
                    RecordNumber = row.McrrIncidentNumber,
                    RowValue = line,
                    RowHash = line.GetSha256Hash(),
                    RowStatusId = _statusService.RowReceived,
                    EndDate = row.ReportDate ?? Constants.MinDate,
                    RowNum = csv.Context.Row
                });
            }

            if (errors.Count == 0)
            {
                Validate(rows, Entities.RockfallReportInit, errors);
            }

            return errors.Count == 0;
        }
    }
}
