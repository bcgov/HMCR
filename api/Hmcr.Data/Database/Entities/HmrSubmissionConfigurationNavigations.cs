using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrSubmissionStream
    {
        public virtual ICollection<HmrSubmissionConfiguration> HmrSubmissionConfigurations { get; set; } =
            new HashSet<HmrSubmissionConfiguration>();
    }

    public partial class HmrActivityCode
    {
        public virtual ICollection<HmrSubmissionConfigRuleActivity> HmrSubmissionConfigRuleActivities { get; set; } =
            new HashSet<HmrSubmissionConfigRuleActivity>();
    }

    public partial class HmrServiceArea
    {
        public virtual ICollection<HmrSubmissionConfigServiceArea> HmrSubmissionConfigServiceAreas { get; set; } =
            new HashSet<HmrSubmissionConfigServiceArea>();
    }
}
