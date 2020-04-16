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
            IContractTermRepository contractRepo, ISubmissionStatusRepository statusRepo, IWorkReportRepository workRptRepo, IFieldValidatorService validator,
            ILogger<WorkReportService> logger, IServiceAreaService saService)
            : base(unitOfWork, streamService, submissionRepo, rowRepo, contractRepo, statusRepo, validator, saService)
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
            var rowNum = 1;

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
                    errors.AddItem("ServiceArea", $"The file contains service area which is not {serviceAreastrings[0]}.");
                    return false;
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();

                submission.SubmissionRows.Add(new SubmissionRowDto
                {
                    RecordNumber = row.RecordNumber,
                    RowValue = line,
                    RowHash = line.GetSha256Hash(),
                    RowStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.Row, RowStatus.RowReceived),
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
