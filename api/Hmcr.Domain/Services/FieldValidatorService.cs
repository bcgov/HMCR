using Hmcr.Model;
using Hmcr.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public FieldValidatorService()
        {
            _rules = new HashSet<FieldValidationRule>();
            LoadUserEntityRules();
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
            _rules.Add(new FieldValidationRule(Entities.User, Fields.Email, FieldTypes.String, true, 1, 100, null, null, null, null, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", null));
            _rules.Add(new FieldValidationRule(Entities.User, Fields.EndDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
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

            if (!rule.Required && val is null)
                return messages;

            string value = Convert.ToString(val);

            if (rule.MinLength != null && rule.MaxLength != null)
            {
                if (value.Length < rule.MinLength || value.Length > rule.MaxLength)
                {
                    messages.Add($"The length of {rule.FieldName} field must be between {rule.MinLength} and {rule.MaxLength}.");
                }
            }

            if (rule.Regex.IsNotEmpty())
            {
                if (!Regex.IsMatch(value, rule.Regex))
                {
                    messages.Add($"{rule.FieldName} field must match the regular expression [{rule.Regex}].");
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

            if (!rule.Required && val is null)
                return messages;

            DateTime value = Convert.ToDateTime(val);

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
