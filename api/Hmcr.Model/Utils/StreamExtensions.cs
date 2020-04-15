using System.IO;

namespace Hmcr.Model.Utils
{
    public static class StreamExtensions
    {
        public static byte[] ToBytes(this Stream stream)
        {
            stream.Position = 0;
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            stream.Position = 0;
            return ms.ToArray();
        }
    }
}
