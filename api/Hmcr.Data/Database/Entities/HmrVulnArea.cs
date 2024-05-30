using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hmcr.Data.Database.Entities
{
    [Table("HMR_SALT_VULNAREA", Schema = "dbo")]
    public class HmrSaltVulnArea
    {
        public decimal VulnerableAreaId { get; set; }
        public decimal SaltReportId { get; set; }
        public HmrSaltReport SaltReport { get; set; } // Navigation property

        [Column("HWY_NO")]
        [StringLength(16)]
        public string HighwayNumber { get; set; }

        [Column("LAT")]
        public decimal? Latitude { get; set; }

        [Column("LONG")]
        public decimal? Longitude { get; set; }

        [Column("FEATURE")]
        [StringLength(255)]
        public string Feature { get; set; }

        [Column("TYPE")]
        [StringLength(255)]
        public string Type { get; set; }

        [Column("PROT_MEASURES")]
        [StringLength(255)]
        public string ProtectionMeasures { get; set; }

        [Column("ENV_MONITORING")]
        public bool? EnvironmentalMonitoring { get; set; }

        [Column("COMMENTS")]
        [StringLength(255)]
        public string Comments { get; set; }

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
