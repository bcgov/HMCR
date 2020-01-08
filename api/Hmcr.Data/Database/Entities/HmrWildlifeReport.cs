﻿using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrWildlifeReport
    {
        public decimal WildlifeRecordId { get; set; }
        public decimal SubmissionObjectId { get; set; }
        public decimal? ValidationStatusId { get; set; }
        public string RecordType { get; set; }
        public decimal ServiceArea { get; set; }
        public DateTime? AccidentDate { get; set; }
        public string TimeOfKill { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string HighwayUniqueNumber { get; set; }
        public string Landmark { get; set; }
        public decimal? StartOffset { get; set; }
        public string NearestTown { get; set; }
        public string WildlifeSign { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Species { get; set; }
        public string Sex { get; set; }
        public string Age { get; set; }
        public string Comment { get; set; }
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

        public virtual HmrServiceArea ServiceAreaNavigation { get; set; }
        public virtual HmrSubmissionObject SubmissionObject { get; set; }
        public virtual HmrSubmissionStatu ValidationStatus { get; set; }
    }
}
