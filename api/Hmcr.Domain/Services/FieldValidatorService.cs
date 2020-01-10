using Hmcr.Model;
using Hmcr.Model.Dtos.CodeLookup;
using Hmcr.Model.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hmcr.Domain.Services
{
    public interface IFieldValidatorService
    {
        void Validate<T>(string entityName, string fieldName, T value, Dictionary<string, List<string>> errors);
        void Validate<T>(string entityName, T entity, Dictionary<string, List<string>> errors, params string[] fieldsToSkip);
        IEnumerable<CodeLookupForValidation> CodeLookup { get; set; }
    }
    public class FieldValidatorService : IFieldValidatorService
    {
        List<FieldValidationRule> _rules;
        RegexDefs _regex;
        public IEnumerable<CodeLookupForValidation> CodeLookup { get; set; }
        public FieldValidatorService(RegexDefs regex)
        {
            _rules = new List<FieldValidationRule>();
            _regex = regex;

            LoadUserEntityRules();
            LoadRoleEntityRules();

            LoadWorkReportD2Rules();
            LoadWorkReportD2BRules();
            LoadWorkReportD3Rules();
            LoadWorkReportD3SiteRules();
            LoadWorkReportD4Rules();
            LoadWorkReportD4SiteRules();

            LoadRockfallReportRules();
            LoadRockfallReportGpsRules();
            LoadRockfallReportLrsRules();

            LoadWildlifeReportRules();
            LoadWildlifeReportGpsRules();
            LoadWildlifeReportLrsRules();
        }
        public IEnumerable<FieldValidationRule> GetFieldValidationRules(string entityName)
        {
            return _rules.Where(x => x.EntityName.ToLowerInvariant() == entityName.ToLowerInvariant());
        }

        private void LoadUserEntityRules()
        {
            _rules.Add(new FieldValidationRule(Entities.User, Fields.Username, FieldTypes.String, true, 1, 32, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.User, Fields.UserType, FieldTypes.String, true, 1, 30, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.User, Fields.EndDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
        }

        private void LoadRoleEntityRules()
        {
            _rules.Add(new FieldValidationRule(Entities.Role, Fields.Name, FieldTypes.String, true, 1, 30, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.Role, Fields.Description, FieldTypes.String, true, 1, 150, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.Role, Fields.EndDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
        }

        private void LoadWorkReportD2Rules()
        {
            //Common for all work reports
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.RecordType, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WrkRptMaintType));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.RecordNumber, FieldTypes.String, true, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.TaskNumber, FieldTypes.String, false, 0, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.ActivityNumber, FieldTypes.String, true, 6, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StartDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.EndDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.Accomplishment, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D7_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.UnitOfMeasure, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.UnitOfMeasure)); 
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.PostedDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.Comments, FieldTypes.String, false, 0, 1024, null, null, null, null, null, null));

            //Highway Unique is not required
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.HighwayUnique, FieldTypes.String, false, 0, 16, null, null, null, null, null, null));

            //GPS info is not required
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StartLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D11_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StartLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D11_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.EndLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D11_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.EndLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D11_6), null));

            //LRS info is not required
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.Landmark, FieldTypes.String, false, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StartOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D7_3), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.EndOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D7_3), null));

            //Structure and Site info are not required
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StructureNumber, FieldTypes.String, false, 0, 5, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.SiteNumber, FieldTypes.String, false, 0, 8, null, null, null, null, _regex.GetRegexInfo(RegexDefs.SiteNumber), null));

            //Value of work is not required
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.ValueOfWork, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D8_2), null));
        }

        private void LoadWorkReportD2BRules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.WorkReportD2).Select(x => x.ShallowCopy(Entities.WorkReportD2B)).ToArray());

            //For location B, Highway Unique is required
            _rules.First(x => x.EntityName == Entities.WorkReportD2B && x.FieldName == Fields.HighwayUnique).Required = true;
        }

        private void LoadWorkReportD3Rules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.WorkReportD2).Select(x => x.ShallowCopy(Entities.WorkReportD3)).ToArray());

            //For D3, Highway Unique, GPS info and value of work are required.
            _rules.First(x => x.EntityName == Entities.WorkReportD3 && x.FieldName == Fields.HighwayUnique).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD3 && x.FieldName == Fields.StartLatitude).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD3 && x.FieldName == Fields.StartLongitude).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD3 && x.FieldName == Fields.EndLatitude).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD3 && x.FieldName == Fields.EndLongitude).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD3 && x.FieldName == Fields.ValueOfWork).Required = true;
        }

        private void LoadWorkReportD3SiteRules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.WorkReportD3).Select(x => x.ShallowCopy(Entities.WorkReportD3Site)).ToArray());

            //For some D3, Structure and Site info are also required
            _rules.First(x => x.EntityName == Entities.WorkReportD3Site && x.FieldName == Fields.StructureNumber).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD3Site && x.FieldName == Fields.SiteNumber).Required = true;
        }

        private void LoadWorkReportD4Rules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.WorkReportD2).Select(x => x.ShallowCopy(Entities.WorkReportD4)).ToArray());

            //For D4, Highway Unique, LRS info and value of work are required.
            _rules.First(x => x.EntityName == Entities.WorkReportD4 && x.FieldName == Fields.HighwayUnique).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD4 && x.FieldName == Fields.Landmark).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD4 && x.FieldName == Fields.StartOffset).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD4 && x.FieldName == Fields.EndOffset).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD4 && x.FieldName == Fields.ValueOfWork).Required = true;
        }

        private void LoadWorkReportD4SiteRules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.WorkReportD4).Select(x => x.ShallowCopy(Entities.WorkReportD4Site)).ToArray());

            //For some D4, Structure and Site info are also required
            _rules.First(x => x.EntityName == Entities.WorkReportD4Site && x.FieldName == Fields.StructureNumber).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD4Site && x.FieldName == Fields.SiteNumber).Required = true;
        }

        public void LoadRockfallReportRules()
        {
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.RecordType, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.F), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.McrrIncidentNumber, FieldTypes.String, true, 0, 12, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.EstimatedRockfallDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.EstimatedRockfallTime, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Time), null));

            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.StartLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D11_6), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.StartLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D11_6), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.EndLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D11_6), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.EndLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D11_6), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.HighwayUniqueNumber, FieldTypes.String, true, 0, 16, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.HighwayUniqueName, FieldTypes.String, true, 0, 255, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.Landmark, FieldTypes.String, false, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.LandMarkName, FieldTypes.String, false, 0, 255, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.StartOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D7_3), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.EndOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D7_3), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.DirectionFromLandmark, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Direction), null));

            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.LocationDescription, FieldTypes.String, false, 0, 4000, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.DitchVolume, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.VolumeRange));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.TravelledLanesVolume, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.VolumeRange));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.OtherVolume, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D6_2), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.HeavyPrecip, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.YN), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.FreezeThaw, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.YN), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.DitchSnowIce, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.YN), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.VehicleDamage, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.YN), null));

            //todo: other ditch volume and other travelled lanes volume

            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.Comments, FieldTypes.String, false, 0, 1024, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.McName, FieldTypes.String, true, 0, 150, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.Name, FieldTypes.String, true, 0, 150, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.McPhoneNumber, FieldTypes.String, true, 0, 12, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.ReportDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));

        }

        public void LoadRockfallReportGpsRules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.RockfallReport).Select(x => x.ShallowCopy(Entities.RockfallReportGps)).ToArray());

            _rules.First(x => x.EntityName == Entities.RockfallReport && x.FieldName == Fields.StartLatitude).Required = true;
            _rules.First(x => x.EntityName == Entities.RockfallReport && x.FieldName == Fields.StartLongitude).Required = true;
            _rules.First(x => x.EntityName == Entities.RockfallReport && x.FieldName == Fields.EndLatitude).Required = true;
            _rules.First(x => x.EntityName == Entities.RockfallReport && x.FieldName == Fields.EndLongitude).Required = true;
        }

        public void LoadRockfallReportLrsRules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.RockfallReport).Select(x => x.ShallowCopy(Entities.RockfallReportLrs)).ToArray());

            _rules.First(x => x.EntityName == Entities.RockfallReport && x.FieldName == Fields.Landmark).Required = true;
            _rules.First(x => x.EntityName == Entities.RockfallReport && x.FieldName == Fields.LandMarkName).Required = true;
            _rules.First(x => x.EntityName == Entities.RockfallReport && x.FieldName == Fields.StartOffset).Required = true;
            _rules.First(x => x.EntityName == Entities.RockfallReport && x.FieldName == Fields.EndOffset).Required = true;
            _rules.First(x => x.EntityName == Entities.RockfallReport && x.FieldName == Fields.DirectionFromLandmark).Required = true;
        }

        public void LoadWildlifeReportRules()
        {
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.RecordType, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WarsRptRecordType));
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.AccidentDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.TimeOfKill, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WarsTime));

            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Latitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D11_6), null));
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Longitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D11_6), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.HighwayUniqueNumber, FieldTypes.String, true, 0, 16, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Landmark, FieldTypes.String, false, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.StartOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D7_3), null));
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.NearestTown, FieldTypes.String, false, 0, 150, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.WildlifeSign, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WarsRptSign)); //LOV
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Quantity, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4), null));

            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Species, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WarSpecies)); //LOV
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Sex, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WarsSex)); //LOV
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Age, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WarsAge)); //LOV
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Comments, FieldTypes.String, false, 0, 1024, null, null, null, null, null, null));
        }

        public void LoadWildlifeReportGpsRules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.WildlifeReport).Select(x => x.ShallowCopy(Entities.WildlifeReportGps)).ToArray());

            _rules.First(x => x.EntityName == Entities.WildlifeReport && x.FieldName == Fields.Latitude).Required = true;
            _rules.First(x => x.EntityName == Entities.WildlifeReport && x.FieldName == Fields.Longitude).Required = true;
        }

        public void LoadWildlifeReportLrsRules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.WildlifeReport).Select(x => x.ShallowCopy(Entities.WildlifeReportLrs)).ToArray());

            _rules.First(x => x.EntityName == Entities.WildlifeReport && x.FieldName == Fields.Landmark).Required = true;
            _rules.First(x => x.EntityName == Entities.WildlifeReport && x.FieldName == Fields.StartOffset).Required = true;
            _rules.First(x => x.EntityName == Entities.WildlifeReport && x.FieldName == Fields.NearestTown).Required = true;
        }

        public void Validate<T>(string entityName, T entity, Dictionary<string, List<string>> errors, params string[] fieldsToSkip)
        {
            var fields = typeof(T).GetProperties();

            foreach (var field in fields)
            {
                if (fieldsToSkip.Any(x => x == field.Name))
                    continue;

                Validate(entityName, field.Name, field.GetValue(entity), errors);
            }
        }

        public void Validate<T>(string entityName, string fieldName, T val, Dictionary<string, List<string>> errors)
        {
            var rule = _rules.FirstOrDefault(r => r.EntityName == entityName && r.FieldName == fieldName);

            if (rule == null)
                return;

            var messages = new List<string>();

            switch (rule.FieldType)
            {
                case FieldTypes.String:
                    messages.AddRange(ValidateStringField(rule, val));
                    break;
                case FieldTypes.Date:
                    messages.AddRange(ValidateDateField(rule, val));
                    break;
                default:
                    throw new NotImplementedException($"Validation for {rule.FieldType} is not implemented.");
            }

            if (messages.Count > 0)
                errors.Add(rule.FieldName, messages);
        }

        private List<string> ValidateStringField<T>(FieldValidationRule rule, T val)
        {
            var messages = new List<string>();

            if (rule.Required && val is null)
            {
                messages.Add($"The {rule.FieldName} field is required.");
                return messages;
            }

            if (!rule.Required && (val is null || val.ToString().IsEmpty()))
                return messages;

            string value = Convert.ToString(val);

            if (rule.Required && value.IsEmpty())
            {
                messages.Add($"The {rule.FieldName} field is required.");
                return messages;
            }

            if (rule.MinLength != null && rule.MaxLength != null)
            {
                if (value.Length < rule.MinLength || value.Length > rule.MaxLength)
                {
                    messages.Add($"The length of {rule.FieldName} field must be between {rule.MinLength} and {rule.MaxLength}.");
                }
            }

            if (rule.Regex != null)
            {
                if (!Regex.IsMatch(value, rule.Regex.Regex))
                {
                    messages.Add($"{rule.FieldName} {rule.Regex.ErrorMessage}.");
                }
            }

            if (rule.CodeSet != null)
            {
                if (!CodeLookup.Any(x => x.CodeSet == rule.CodeSet && x.CodeValue.ToLowerInvariant() == value.ToLowerInvariant()))
                {
                    messages.Add($"Invalid value. [{value}] doesn't exist in the code set {rule.CodeSet}.");
                }
            }

            return messages;
        }

        private List<string> ValidateDateField<T>(FieldValidationRule rule, T val)
        {
            var messages = new List<string>();

            if (rule.Required && val is null)
            {
                messages.Add($"{rule.FieldName} field is required.");
                return messages;
            }

            if (!rule.Required && (val is null || val.ToString().IsEmpty()))
                return messages;

            var (parsed, parsedDate) = DateUtils.ParseDate(val);

            if (!parsed)
            {
                messages.Add($"Cannot convert {rule.FieldName} field to date");
                return messages;
            }

            var value = parsedDate;

            if (rule.MinDate != null && rule.MaxDate != null)
            {
                if (value < rule.MinDate || value > rule.MaxDate)
                {
                    messages.Add($"The length of {rule.FieldName} must be between {rule.MinDate} and {rule.MaxDate}.");
                }
            }

            return messages;
        }
    }
}
