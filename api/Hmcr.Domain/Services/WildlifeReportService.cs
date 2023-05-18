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
using System.Globalization;
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
            IContractTermRepository contractRepo, ISubmissionStatusService statusService, IWildlifeReportRepository wildlifeRepo, IFieldValidatorService validator,
            ILogger<WildlifeReportService> logger, IServiceAreaService saService)
            : base(unitOfWork, streamService, submissionRepo, rowRepo, contractRepo, statusService, validator, saService, logger)
        {
            TableName = TableNames.WildlifeReport;
            HasRowIdentifier = false;
            DateFieldName = Fields.AccidentDate;
            _wildlifeRepo = wildlifeRepo;
            _logger = logger;
        }
        protected override async Task<bool> ParseRowsAsync(SubmissionObjectCreateDto submission, TextReader textReader, Dictionary<string, List<string>> errors)
        {
            using var csv = new CsvReader(textReader, CultureInfo.InvariantCulture);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WildlifeRptInitCsvDtoMap>();

            var serviceAreastrings = ConvertServiceAreaToStrings(submission.ServiceAreaNumber);
            var serviceArea = await _saService.GetServiceAreaByServiceAreaNumberAsyc(submission.ServiceAreaNumber);

            var headerValidated = false;
            var rows = new List<WildlifeRptInitCsvDto>();

            while (csv.Read())
            {
                WildlifeRptInitCsvDto row = null;

                try
                {
                    row = csv.GetRecord<WildlifeRptInitCsvDto>();

                    if (!headerValidated)
                    {
                        if (!CheckCommonMandatoryFields(csv.Context.HeaderRecord, WildlifeReportHeaders.MandatoryFields, errors))
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

                if (!serviceAreastrings.Contains(row.ServiceArea))
                {
                    errors.AddItem("ServiceArea", $"The file contains service area which is not {serviceAreastrings[0]}.");
                    return false;
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();

                submission.SubmissionRows.Add(new SubmissionRowDto
                {
                    RecordNumber = null,
                    RowValue = line,
                    RowHash = line.GetSha256Hash(),
                    RowStatusId = _statusService.RowReceived,
                    EndDate = row.AccidentDate ?? Constants.MinDate,
                    RowNum = csv.Context.Row
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
