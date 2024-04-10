using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hmcr.Data.Database.Entities
{
    public class HmrSaltStockpile
    {
        public decimal StockPileId { get; set; } // Primary Key
        public decimal SaltReportId { get; set; } // Foreign Key
        public HmrSaltReport SaltReport { get; set; } // Navigation property

        [Column("SITE_NAME")]
        public string SiteName { get; set; }

        [Column("MOTI_OWNED")]
        public bool? MotiOwned { get; set; }

        [Column("ROAD_SALT_STOCKPILES_TOTAL")]
        public int? RoadSaltStockpilesTotal { get; set; }

        [Column("ROAD_SALT_ON_IMPERMEABLE_SURFACE")]
        public int? RoadSaltOnImpermeableSurface { get; set; }

        [Column("ROAD_SALT_UNDER_PERMANENT_ROOF")]
        public int? RoadSaltUnderPermanentRoof { get; set; }

        [Column("ROAD_SALT_UNDER_TARP")]
        public int? RoadSaltUnderTarp { get; set; }

        [Column("TREATED_ABRASIVES_STOCKPILES_TOTAL")]
        public int? TreatedAbrasivesStockpilesTotal { get; set; }

        [Column("TREATED_ABRASIVES_ON_IMPERMEABLE_SURFACE")]
        public int? TreatedAbrasivesOnImpermeableSurface { get; set; }

        [Column("TREATED_ABRASIVES_UNDER_PERMANENT_ROOF")]
        public int? TreatedAbrasivesUnderPermanentRoof { get; set; }

        [Column("TREATED_ABRASIVES_UNDER_TARP")]
        public int? TreatedAbrasivesUnderTarp { get; set; }

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
