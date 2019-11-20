using Hmcr.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hmcr.Data.Repositories
{
    public interface IFieldValidationRepository
    {

    }

    public class FieldValidationRepository
    {
        IEnumerable<FieldValidationRule> _fieldValidationRules;

        public FieldValidationRepository()
        {
            _fieldValidationRules = new List<FieldValidationRule>();
        }
        public IEnumerable<FieldValidationRule> GetFieldValidationRules(string entityName)
        {
            return _fieldValidationRules.Where(x => x.EntityName.ToLowerInvariant() == entityName.ToLowerInvariant());
        }

        private void LoadUserDtoRules()
        {

        }

    }
}
