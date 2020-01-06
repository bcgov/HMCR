using Hmcr.Model;
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
    }
    public class FieldValidatorService : IFieldValidatorService
    {
        HashSet<FieldValidationRule> _rules;
        RegexDefs _regex;
        
        public FieldValidatorService(RegexDefs regex)
        {
            _rules = new HashSet<FieldValidationRule>();
            _regex = regex;

            LoadUserEntityRules();
            LoadRoleEntityRules();
            LoadWorkReportD2Rules();
            LoadWorkReportD2BRules();
            LoadWorkReportD3Rules();
            LoadWorkReportD3SiteRules();
            LoadWorkReportD4Rules();
            LoadWorkReportD4SiteRules();

        }
        public IEnumerable<FieldValidationRule> GetFieldValidationRules(string entityName)
        {
            return _rules.Where(x => x.EntityName.ToLowerInvariant() == entityName.ToLowerInvariant());
        }

        private void LoadUserEntityRules()
        {
            _rules.Add(new FieldValidationRule(Entities.User, Fields.Username, FieldTypes.String, true, 1, 32, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.User, Fields.UserType, FieldTypes.String, true, 1, 30, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.User, Fields.FirstName, FieldTypes.String, true, 1, 150, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.User, Fields.LastName, FieldTypes.String, true, 1, 150, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.User, Fields.Email, FieldTypes.String, true, 1, 100, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Email), null));
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
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.RecordType, FieldTypes.String, true, 1, 1, null, null, null, null, _regex.GetRegexInfo(RegexDefs.QREA), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.RecordNumber, FieldTypes.String, true, 1, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.TaskNumber, FieldTypes.String, false, 0, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.ActivityNumber, FieldTypes.String, true, 6, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StartDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.EndDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.Accomplishment, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.UnitOfMeasure, FieldTypes.String, true, 1, 3, null, null, null, null, null, null)); //todo lookup
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.PostedDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.HighwayUnique, FieldTypes.String, false, 0, 16, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StartLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StartLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.EndLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.EndLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.Landmark, FieldTypes.String, false, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StartOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4_3), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.EndOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4_3), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StructureNumber, FieldTypes.String, false, 0, 5, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.SiteNumber, FieldTypes.String, false, 0, 8, null, null, null, null, _regex.GetRegexInfo(RegexDefs.SiteNumber), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.ValueOfWork, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Dollar6_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.Comments, FieldTypes.String, false, 0, 1024, null, null, null, null, null, null));
        }
        private void LoadWorkReportD2BRules()
        {
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.RecordType, FieldTypes.String, true, 1, 1, null, null, null, null, _regex.GetRegexInfo(RegexDefs.QREA), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.RecordNumber, FieldTypes.String, true, 1, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.TaskNumber, FieldTypes.String, false, 0, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.ActivityNumber, FieldTypes.String, true, 6, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.StartDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.EndDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.Accomplishment, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.UnitOfMeasure, FieldTypes.String, true, 1, 3, null, null, null, null, null, null)); //todo lookup
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.PostedDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.HighwayUnique, FieldTypes.String, true, 0, 16, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.StartLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.StartLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.EndLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.EndLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.Landmark, FieldTypes.String, false, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.StartOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4_3), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.EndOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4_3), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.StructureNumber, FieldTypes.String, false, 0, 5, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.SiteNumber, FieldTypes.String, false, 0, 8, null, null, null, null, _regex.GetRegexInfo(RegexDefs.SiteNumber), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.ValueOfWork, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Dollar6_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2B, Fields.Comments, FieldTypes.String, false, 0, 1024, null, null, null, null, null, null));
        }

        private void LoadWorkReportD3Rules()
        {
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.RecordType, FieldTypes.String, true, 1, 1, null, null, null, null, _regex.GetRegexInfo(RegexDefs.QREA), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.RecordNumber, FieldTypes.String, true, 1, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.TaskNumber, FieldTypes.String, false, 0, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.ActivityNumber, FieldTypes.String, true, 6, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.StartDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.EndDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.Accomplishment, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.UnitOfMeasure, FieldTypes.String, true, 1, 3, null, null, null, null, null, null)); //todo lookup
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.PostedDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.HighwayUnique, FieldTypes.String, true, 0, 16, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.StartLatitude, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.StartLongitude, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.EndLatitude, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.EndLongitude, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.Landmark, FieldTypes.String, false, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.StartOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4_3), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.EndOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4_3), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.StructureNumber, FieldTypes.String, false, 0, 5, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.SiteNumber, FieldTypes.String, false, 0, 8, null, null, null, null, _regex.GetRegexInfo(RegexDefs.SiteNumber), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.ValueOfWork, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Dollar6_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3, Fields.Comments, FieldTypes.String, false, 0, 1024, null, null, null, null, null, null));
        }

        private void LoadWorkReportD3SiteRules()
        {
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.RecordType, FieldTypes.String, true, 1, 1, null, null, null, null,_regex.GetRegexInfo(RegexDefs.QREA), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.RecordNumber, FieldTypes.String, true, 1, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.TaskNumber, FieldTypes.String, false, 0, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.ActivityNumber, FieldTypes.String, true, 6, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.StartDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.EndDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.Accomplishment, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.UnitOfMeasure, FieldTypes.String, true, 1, 3, null, null, null, null, null, null)); //todo lookup
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.PostedDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.HighwayUnique, FieldTypes.String, true, 0, 16, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.StartLatitude, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.StartLongitude, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.EndLatitude, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.EndLongitude, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.Landmark, FieldTypes.String, false, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.StartOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4_3), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.EndOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4_3), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.StructureNumber, FieldTypes.String, true, 0, 5, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.SiteNumber, FieldTypes.String, true, 0, 8, null, null, null, null, _regex.GetRegexInfo(RegexDefs.SiteNumber), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.ValueOfWork, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Dollar6_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD3Site, Fields.Comments, FieldTypes.String, false, 0, 1024, null, null, null, null, null, null));
        }

        private void LoadWorkReportD4Rules()
        {
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.RecordType, FieldTypes.String, true, 1, 1, null, null, null, null,_regex.GetRegexInfo(RegexDefs.QREA), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.RecordNumber, FieldTypes.String, true, 1, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.TaskNumber, FieldTypes.String, false, 0, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.ActivityNumber, FieldTypes.String, true, 6, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.StartDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.EndDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.Accomplishment, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.UnitOfMeasure, FieldTypes.String, true, 1, 3, null, null, null, null, null, null)); //todo lookup
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.PostedDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.HighwayUnique, FieldTypes.String, true, 0, 16, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.StartLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.StartLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.EndLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.EndLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.Landmark, FieldTypes.String, true, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.StartOffset, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4_3), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.EndOffset, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4_3), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.StructureNumber, FieldTypes.String, false, 0, 5, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.SiteNumber, FieldTypes.String, false, 0, 8, null, null, null, null, _regex.GetRegexInfo(RegexDefs.SiteNumber), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.ValueOfWork, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Dollar6_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4, Fields.Comments, FieldTypes.String, false, 0, 1024, null, null, null, null, null, null));
        }

        private void LoadWorkReportD4SiteRules()
        {
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.RecordType, FieldTypes.String, true, 1, 1, null, null, null, null,_regex.GetRegexInfo(RegexDefs.QREA), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.RecordNumber, FieldTypes.String, true, 1, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.TaskNumber, FieldTypes.String, false, 0, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.ActivityNumber, FieldTypes.String, true, 6, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.StartDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.EndDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.Accomplishment, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.UnitOfMeasure, FieldTypes.String, true, 1, 3, null, null, null, null, null, null)); //todo lookup
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.PostedDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.HighwayUnique, FieldTypes.String, true, 0, 16, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.StartLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.StartLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.EndLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.EndLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D5_6), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.Landmark, FieldTypes.String, true, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.StartOffset, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4_3), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.EndOffset, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.D4_3), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.StructureNumber, FieldTypes.String, true, 0, 5, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.SiteNumber, FieldTypes.String, true, 0, 8, null, null, null, null, _regex.GetRegexInfo(RegexDefs.SiteNumber), null));

            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.ValueOfWork, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Dollar6_2), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD4Site, Fields.Comments, FieldTypes.String, false, 0, 1024, null, null, null, null, null, null));
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

            //ToDo: look up validation

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
