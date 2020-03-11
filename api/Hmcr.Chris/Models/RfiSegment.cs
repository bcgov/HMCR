namespace Hmcr.Chris.Models
{
    public class RfiSegment
    {
        public RecordDimension Dimension { get; set; }
        /// <summary>
        /// Length in KM
        /// </summary>
        public decimal Length { get; set; }
        public string Descr { get; set; }
    }
}
