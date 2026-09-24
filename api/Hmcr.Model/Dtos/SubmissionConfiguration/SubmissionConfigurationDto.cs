using System;
using System.Collections.Generic;

namespace Hmcr.Model.Dtos.SubmissionConfiguration
{
    public class SubmissionConfigurationDto
    {
        public SubmissionConfigurationDto()
        {
            Windows = new List<SubmissionConfigurationWindowDto>();
            Rules = new List<SubmissionConfigurationRuleDto>();
            Audiences = new List<SubmissionConfigurationAudienceDto>();
            ServiceAreaNumbers = new List<decimal>();
        }

        public decimal SubmissionConfigurationId { get; set; }
        public string ConfigurationKey { get; set; }
        public string Name { get; set; }
        public decimal SubmissionStreamId { get; set; }
        public string SubmissionStreamName { get; set; }
        public bool IsActive { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public string TimeZone { get; set; }
        public string ScopeType { get; set; }
        public string CurrentState { get; set; }
        public string GeneratedNotice { get; set; }
        public string NoticeSeverity { get; set; }
        public DateTime? CurrentWindowStartDate { get; set; }
        public DateTime? CurrentWindowEndDate { get; set; }
        public DateTime? NextBlackoutStartDate { get; set; }
        public DateTime? NextBlackoutEndDate { get; set; }
        public long ConcurrencyControlNumber { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTimestamp { get; set; }
        public IList<SubmissionConfigurationWindowDto> Windows { get; set; }
        public IList<SubmissionConfigurationRuleDto> Rules { get; set; }
        public IList<SubmissionConfigurationAudienceDto> Audiences { get; set; }
        public IList<decimal> ServiceAreaNumbers { get; set; }
    }

    public class SubmissionConfigurationWindowDto
    {
        public decimal SubmissionConfigWindowId { get; set; }
        public string WindowType { get; set; }
        public int StartMonth { get; set; }
        public int StartDay { get; set; }
        public int EndMonth { get; set; }
        public int EndDay { get; set; }
    }

    public class SubmissionConfigurationRuleDto
    {
        public SubmissionConfigurationRuleDto()
        {
            Activities = new List<SubmissionConfigurationActivityDto>();
        }

        public decimal SubmissionConfigRuleId { get; set; }
        public string RuleType { get; set; }
        public string DisplayLabel { get; set; }
        public string ComparisonOperator { get; set; }
        public decimal ThresholdValue { get; set; }
        public string UnitOfMeasure { get; set; }
        public IList<SubmissionConfigurationActivityDto> Activities { get; set; }
    }

    public class SubmissionConfigurationActivityDto
    {
        public decimal ActivityCodeId { get; set; }
        public string ActivityNumber { get; set; }
        public string ActivityName { get; set; }
    }

    public class SubmissionConfigurationAudienceDto
    {
        public string AudienceType { get; set; }
        public string AudienceValue { get; set; }
    }
}
