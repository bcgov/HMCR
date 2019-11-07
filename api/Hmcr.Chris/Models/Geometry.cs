namespace Hmcr.Chris.Models
{
    public class Geometry<T>
    {
        public string type { get; set; }
        public T coordinates { get; set; }
    }
}
