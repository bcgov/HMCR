using CsvHelper;
using CsvHelper.TypeConversion;
using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Domain.Services.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Dtos.WorkReport;
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
    public interface IWorkReportService
    {
        Task<(Dictionary<string, List<string>> errors, List<string> resubmittedRecordNumbers)> CheckResubmitAsync(FileUploadDto upload);
        Task<(decimal submissionObjectId, Dictionary<string, List<string>> errors)> CreateReportAsync(FileUploadDto upload);
    }
    public class WorkReportService : ReportServiceBase, IWorkReportService
    {
        private IWorkReportRepository _workRptRepo;
        private ILogger<WorkReportService> _logger;

        public WorkReportService(IUnitOfWork unitOfWork, 
            ISubmissionStreamService streamService, ISubmissionObjectRepository submissionRepo, ISumbissionRowRepository rowRepo, 
            IContractTermRepository contractRepo, ISubmissionStatusService statusService, IWorkReportRepository workRptRepo, IFieldValidatorService validator,
            ILogger<WorkReportService> logger, IServiceAreaService saService)
            : base(unitOfWork, streamService, submissionRepo, rowRepo, contractRepo, statusService, validator, saService, logger)
        {
            TableName = TableNames.WorkReport;
            HasRowIdentifier = true;
            RecordNumberFieldName = Fields.RecordNumber;
            DateFieldName = Fields.EndDate;
            _workRptRepo = workRptRepo;
            _logger = logger;
        }

        protected override async Task<bool> ParseRowsAsync(SubmissionObjectCreateDto submission, TextReader textReader, Dictionary<string, List<string>> errors)
        {
            using var csv = new CsvReader(textReader, CultureInfo.InvariantCulture);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WorkRptInitCsvDtoMap>();

            var serviceAreastrings = ConvertServiceAreaToStrings(submission.ServiceAreaNumber);
            var serviceArea = await _saService.GetServiceAreaByServiceAreaNumberAsyc(submission.ServiceAreaNumber);

            var headerValidated = false;
            var rows = new List<WorkRptInitCsvDto>();

            while (csv.Read())
            {
                WorkRptInitCsvDto row = null;

                try
                {
                    row = csv.GetRecord<WorkRptInitCsvDto>();

                    if (!headerValidated)
                    {
                        if (!CheckCommonMandatoryFields(csv.Context.HeaderRecord, WorkReportHeaders.MandatoryFields, errors))
                        {
                            return false;
                        }
                        else
                        {
                            headerValidated = true;
                        }
                    }

                    if (row == null)
                    {
                        errors.AddItem("File", "Row is empty. Please make sure that the report isn't empty, and doesn't have empty data rows");
                        break;
                    }

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

                // TH-106937 Spatial validation step for start and end coordinates
                // Start coordinate is required. Ensure end coordinate exists otherwise treat spatial data as a point.
                if (!string.IsNullOrEmpty(row.EndLatitude) && !string.IsNullOrEmpty(row.EndLongitude))
                {
                    var distance = ParseAndCalculateDistance(row.StartLatitude, row.StartLongitude, row.EndLatitude, row.EndLongitude);
                    
                    if (distance <= Constants.CoordsMinimumDistance) {
                        errors.AddItem("Coordinates", "Start and end coordinates obtained from Latitudes and Longitudes must be greater than 5 meters for all rows");
                        break;
                    }
                }

                if (!serviceAreastrings.Contains(row.ServiceArea))
                {
                    errors.AddItem(Fields.ServiceArea, $"The file contains service area which is not {serviceAreastrings[0]}.");
                    return false;
                }

                if (row.RecordNumber.IsEmpty())
                {
                    errors.AddItem(Fields.RecordNumber, $"Record Number is missing for row [{csv.Context.Row}].");
                    return false;
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();

                submission.SubmissionRows.Add(new SubmissionRowDto
                {
                    RecordNumber = row.RecordNumber,
                    RowValue = line,
                    RowHash = line.GetSha256Hash(),
                    RowStatusId = _statusService.RowReceived,
                    EndDate = row.EndDate ?? Constants.MinDate,
                    RowNum = csv.Context.Row
                });
            }

            if (errors.Count == 0)
            {
                Validate(rows, Entities.WorkReportInit, errors);
            }

            return errors.Count == 0;
        }
    }
}
