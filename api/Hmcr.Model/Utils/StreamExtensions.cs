using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hmcr.Model.Utils
{
    public static class StreamExtensions
    {
        public static byte[] ToBytes(this Stream stream)
        {
            stream.Position = 0;
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
