using System;

namespace Hmcr.Data.Database.Entities
{
    public class HmrSaltStockpile
    {
        public int StockPileId { get; set; } // Primary Key
        public int SaltReportId { get; set; } // Foreign Key
        public HmrSaltReport SaltReport { get; set; } // Navigation property
        public string? SiteName { get; set; }
        public bool? MotiOwned { get; set; }
        public int? RoadSaltStockpilesTotal { get; set; }
        public int? RoadSaltOnImpermeableSurface { get; set; }
        public int? RoadSaltUnderPermanentRoof { get; set; }
        public int? RoadSaltUnderTarp { get; set; }
        public int? TreatedAbrasivesStockpilesTotal { get; set; }
        public int? TreatedAbrasivesOnImpermeableSurface { get; set; }
        public int? TreatedAbrasivesUnderPermanentRoof { get; set; }
        public int? TreatedAbrasivesUnderTarp { get; set; }
    }
}
