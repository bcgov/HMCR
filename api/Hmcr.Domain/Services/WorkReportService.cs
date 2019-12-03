using CsvHelper;
using CsvHelper.Configuration;
using Hmcr.Data.Database;
using Hmcr.Model;
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
        Task<(decimal SubmissionObjectId, Dictionary<string, List<string>> Errors)> PerformInitialValidation(WorkRptUploadDto upload);
    }
    public class WorkReportService : IWorkReportService
    {
        private IUnitOfWork _unitOfWork;
        private HmcrCurrentUser _currentUser;
        private IFieldValidatorService _validator;

        public WorkReportService(IUnitOfWork unitOfWork, HmcrCurrentUser currentUser, IFieldValidatorService validator)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _validator = validator;
        }
        public async Task<(decimal SubmissionObjectId, Dictionary<string, List<string>> Errors)> PerformInitialValidation(WorkRptUploadDto upload)
        {
            var errors = new Dictionary<string, List<string>>();

            //validate file size
            //get the party for the service area for the month (use enddate in the file?)

            var serviceArea = _currentUser.UserInfo.ServiceAreas.FirstOrDefault(x => x.ServiceAreaNumber == upload.ServiceAreaNumber);

            if (serviceArea == null)
            {
                errors.AddItem("SerivceArea", $"The user has no access to the service area {upload.ServiceAreaNumber}.");
                return (0, errors);
            }

            using var stream = upload.ReportFile.OpenReadStream();
            using var text = new StreamReader(stream, Encoding.UTF8);
            using var csv = new CsvReader(text);

            csv.Configuration.PrepareHeaderForMatch = (string header, int index) => Regex.Replace(header.ToLower(), @"\s", string.Empty);
            csv.Configuration.CultureInfo = CultureInfo.GetCultureInfo("en-CA");
            csv.Configuration.TrimOptions = TrimOptions.Trim;
            csv.Configuration.HeaderValidated = (bool valid, string[] column, int row, ReadingContext context) =>
            {
                if (valid) return;

                errors.AddItem($"{column[0]}", $"The header [{column[0].WordToWords()}] is missing.");
            };

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

                if (row.ServiceArea != serviceArea.ServiceAreaNumber.ToString())
                {
                    errors.AddItem("ServiceArea", $"The file contains service area which is not {serviceArea.ServiceAreaName} ({serviceArea.ServiceAreaNumber}).");
                    return (0, errors);
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();
                //todo: calculate hash and see if there's any existing record with the same hash
            }

            if (errors.Count > 0)
                return (0, errors);

            await Task.CompletedTask;

            return (1, null);
        }
    }
}
