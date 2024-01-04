using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Hmcr.Data.Database.Entities
{
    public class HmrSaltReport
    {
        public int SaltReportId { get; set; }
        public string ServiceArea { get; set; }
        public string ContactName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string PlanDeveloped { get; set; }
        public string PlanReviewed { get; set; }
        public string PlanUpdated { get; set; }
        public string ManagerTraining { get; set; }
        public string SupervisorTraining { get; set; }
        public string OperatorTraining { get; set; }
        public string MechanicalTraining { get; set; }
        public string PatrollerTraining { get; set; }
        public string MaterialStorageIdentified { get; set; }
        public string MaterialStorageAchieved { get; set; }
        public string SaltApplicationIdentified { get; set; }
        public string SaltApplicationAchieved { get; set; }
        // Default
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
        public DateTime? DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime? DbAuditLastUpdateTimestamp { get; set; }
    }
}
