using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model
{
    public class EmailBody
    {
        public string TextBody { get; set; }
        public string HtmlBody { get; set; }

        public EmailBody()
        {
            SetTextBody();
            SetHtmlBody();
        }

        private void SetTextBody()
        {
            var textBody = new StringBuilder();

            textBody.AppendLine("Hello,");
            textBody.AppendLine("");
            textBody.AppendLine("A submission (Submission# {0}) was made for maintenance contract reporting.");
            textBody.AppendLine("");
            textBody.AppendLine("Please check the status of the submission at {1} to see if there are errors within the submission. Records that do not pass data validation are not accepted.");
            textBody.AppendLine("");
            textBody.AppendLine("Kindly correct and re-submit the erroneous records only, to avoid over-writing records that are error free.");
            textBody.AppendLine("");
            textBody.AppendLine("Please DO NOT REPLY to this email. If you have questions please contact the appropriate district office.");
            textBody.AppendLine("");
            textBody.AppendLine("Sincerely,");
            textBody.AppendLine("Maintenance Contract Reporting System");
            textBody.AppendLine("Ministry of Transportation and Infrastructure");

            TextBody = textBody.ToString();
        }

        private void SetHtmlBody()
        {
            var htmlBody = new StringBuilder();

            htmlBody.AppendLine("<p>Hello,</p>");
            htmlBody.AppendLine("<p>A submission (Submission# {0}) was made for maintenance contract reporting.</P>");
            htmlBody.AppendLine("<p>Please check the status of the submission at {1} to see if there are errors within the submission. Records that do not pass data validation are not accepted.</p>");
            htmlBody.AppendLine("<p>Kindly correct and re-submit the erroneous records only, to avoid over-writing records that are error free.</p>");
            htmlBody.AppendLine("<p>Please <b>DO NOT REPLY</b> to this email. If you have questions please contact the appropriate district office.</p>");
            htmlBody.AppendLine("Sincerely,<br/>");
            htmlBody.AppendLine("Maintenance Contract Reporting System<br/>");
            htmlBody.AppendLine("Ministry of Transportation and Infrastructure");

            HtmlBody = htmlBody.ToString();
        }
    }
}
