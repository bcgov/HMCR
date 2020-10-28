﻿using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrActivityCode
    {
        public HmrActivityCode()
        {
            HmrServiceAreaActivities = new HashSet<HmrServiceAreaActivity>();
        }

        public decimal ActivityCodeId { get; set; }
        public string ActivityNumber { get; set; }
        public string ActivityName { get; set; }
        public string UnitOfMeasure { get; set; }
        public string MaintenanceType { get; set; }
        public decimal LocationCodeId { get; set; }
        public string FeatureType { get; set; }
        public string SpThresholdLevel { get; set; }
        public bool? IsSiteNumRequired { get; set; }
        public string ActivityApplication { get; set; }
        public DateTime? EndDate { get; set; }
        public long ConcurrencyControlNumber { get; set; }
        public string AppCreateUserid { get; set; }
        public DateTime AppCreateTimestamp { get; set; }
        public Guid AppCreateUserGuid { get; set; }
        public string AppCreateUserDirectory { get; set; }
        public string AppLastUpdateUserid { get; set; }
        public DateTime AppLastUpdateTimestamp { get; set; }
        public Guid AppLastUpdateUserGuid { get; set; }
        public string AppLastUpdateUserDirectory { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }
        public decimal RoadClassRule { get; set; }
        public decimal RoadLengthRule { get; set; }
        public decimal SurfaceTypeRule { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public int? ReportingFrequency { get; set; }

        public virtual HmrLocationCode LocationCode { get; set; }
        public virtual HmrActivityCodeRule RoadClassRuleNavigation { get; set; }
        public virtual HmrActivityCodeRule RoadLengthRuleNavigation { get; set; }
        public virtual HmrActivityCodeRule SurfaceTypeRuleNavigation { get; set; }
        public virtual ICollection<HmrServiceAreaActivity> HmrServiceAreaActivities { get; set; }
    }
}
