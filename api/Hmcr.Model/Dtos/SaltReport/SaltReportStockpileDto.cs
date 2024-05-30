namespace Hmcr.Model.Dtos.SaltReport
{
    public class StockpileDto {
        public decimal StockPileId { get; set; }
        public decimal SaltReportId { get; set; } // Foreign Key
        public string SiteName { get; set; }
        public bool? MotiOwned { get; set; }
        public TotalDto RoadSalts { get; set; }
        public TotalDto TreatedAbrasives { get; set; }
    }

    public class TotalDto {
        public int? StockpilesTotal { get; set; }
        public int? OnImpermeableSurface { get; set; }
        public int? UnderPermanentRoof { get; set; }
        public int? UnderTarp { get; set; }
    }
}
