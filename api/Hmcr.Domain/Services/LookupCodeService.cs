using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Utils;
using System;
using System.Linq;

namespace Hmcr.Domain.Services
{
    public interface ILookupCodeService
    {
        SpThresholdLevel GetThresholdLevel(string level);
    }
    public class LookupCodeService : ILookupCodeService
    {
        private IFieldValidatorService _validator;

        public LookupCodeService(IFieldValidatorService validator)
        {
            _validator = validator;
        }

        public SpThresholdLevel GetThresholdLevel(string level)
        {
            if (level.IsEmpty())
            {
                level = ThresholdSpLevels.Level1;
            }

            var threshold = _validator.CodeLookup.FirstOrDefault(x => x.CodeSet == CodeSet.ThresholdSp && x.CodeName == level);

            if (threshold == null)
            {
                throw new Exception($"Cannot find THRSHLD_SP_VAR value {level}");
            }           

            return new SpThresholdLevel(level, threshold.CodeValue);
        }
    }
}
