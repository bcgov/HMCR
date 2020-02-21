namespace Hmcr.Chris.Models
{
    public class Point
    {
        public decimal[] Coordinates { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }

        /// <summary>
        /// Longitude, Latitude
        /// </summary>
        /// <param name="coordinates"></param>
        public Point(decimal[] coordinates)
        {
            Coordinates = coordinates;

            Longitude = coordinates[0];
            Latitude = coordinates[1];
        }

        public Point(decimal longitude, decimal latitude)
        {
            Longitude = longitude;
            Latitude = latitude;

            Coordinates = new decimal[2];

            Coordinates[0] = Longitude;
            Coordinates[1] = Latitude;
        }
    }
}
