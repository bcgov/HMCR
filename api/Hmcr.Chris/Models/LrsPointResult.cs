namespace Hmcr.Chris.Models
{
    public class LrsPointResult
    {
        public decimal Offset { get; set; }
        public decimal Variance { get; set; }
        public Point SnappedPoint { get; set; }

        public LrsPointResult()
        {

        }
        public LrsPointResult(decimal offset, decimal variance, Point snappedPoint)
        {
            Offset = offset;
            Variance = variance;
            SnappedPoint = snappedPoint;
        }
    }
}
