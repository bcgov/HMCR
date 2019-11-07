namespace Hmcr.Chris.Models
{
    public class Feature<T>
    {
        public string type { get; set; }
        public Geometry<T> geometry { get; set; }
        public Property properties { get; set; }
    }
}
