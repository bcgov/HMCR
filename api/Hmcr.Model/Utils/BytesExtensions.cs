using System;

namespace Hmcr.Model.Utils
{
    public static class BytesExtensions
    {
        public static string GetSha256Hash(this byte[] bytes)
        {
            if (bytes == null)
                return string.Empty;

            using var sha = new System.Security.Cryptography.SHA256Managed();
            byte[] hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}
