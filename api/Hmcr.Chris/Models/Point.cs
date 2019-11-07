namespace Hmcr.Chris.Models
{
    public class Point
    {
        public decimal[] Coordinates { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }

        public Point(decimal[] coordinates)
        {
            Coordinates = coordinates;

            Longitude = coordinates[0];
            Latitude = coordinates[1];
        }
    }
}
