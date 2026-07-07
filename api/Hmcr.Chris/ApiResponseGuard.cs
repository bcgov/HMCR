using System;
using System.Text.RegularExpressions;

namespace Hmcr.Chris
{
    /// <summary>
    /// CHRIS/OAS and GeoServer return HTML or XML error pages (login pages, proxy errors,
    /// GeoServer service exceptions) with a success status code in some failure modes.
    /// Feeding those into the JSON deserializer produced the cryptic error
    /// "'&lt;' is an invalid start of a value", which failed submissions with 'Unexpected Error'.
    /// This guard turns them into a descriptive exception instead.
    /// </summary>
    internal static class ApiResponseGuard
    {
        public static void EnsureJsonResponse(string content, string operation)
        {
            if (content == null || !content.TrimStart().StartsWith("<"))
                return;

            throw new Exception($"The mapping service returned an error page instead of data while {operation}. " +
                "This usually means the service is temporarily unavailable - please submit the file again later. " +
                $"Service response: {ExtractText(content)}");
        }

        /// <summary>
        /// Strips markup and collapses whitespace so error pages read as a short plain-text snippet.
        /// </summary>
        public static string ExtractText(string content)
        {
            if (content == null)
                return "";

            var text = Regex.Replace(content, "<[^>]+>", " ");
            text = Regex.Replace(text, @"\s+", " ").Trim();

            if (text.Length > 300)
                text = text.Substring(0, 300) + "...";

            return text;
        }
    }
}
