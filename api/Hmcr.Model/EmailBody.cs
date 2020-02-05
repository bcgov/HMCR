using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Hmcr.Model
{
    public class EmailBody
    {
        public string SuccessHtmlBody { get; private set; }
        public string ErrorHtmlBody { get; private set; }

        public EmailBody()
        {
            SuccessHtmlBody = SetHtmlBody(true);
            ErrorHtmlBody = SetHtmlBody(false);
        }

        private string SetHtmlBody(bool success)
        {
            var correctionMessage = success ? "" : " Kindly correct the identified errors and re-submit the file (with all the records) again.";
            var htmlBody = new StringBuilder();

            htmlBody.Append("Hello,");
            htmlBody.Append("<br/>");

            htmlBody.Append("<br/>");

            htmlBody.Append("The following submission was received for maintenance contract reporting:");
            htmlBody.Append("<br/>");

            htmlBody.Append("<br/>");

            htmlBody.Append("<ul>");
            htmlBody.Append("<li>File Name: {0}");
            htmlBody.Append("<li>File Type: {1}");
            htmlBody.Append("<li>Service Area: {2}");
            htmlBody.Append("<li>Submission Date: {3}");
            htmlBody.Append("<li>Submission #: {4}");
            htmlBody.Append("<li>Total Number of Records: {5}");
            htmlBody.Append("<li>Number of Records with Errors: {6}");
            htmlBody.Append("</ul>");

            htmlBody.Append("<br/>");

            htmlBody.Append("Please check the status of the submission at {7}.");
            htmlBody.Append(correctionMessage);
            htmlBody.Append("<br/>");

            htmlBody.Append("<br/>");

            htmlBody.Append("Please <b>DO NOT REPLY</b> to this email. If you have questions please contact the appropriate district office.");
            htmlBody.Append("<br/>");

            htmlBody.Append("<br/>");

            htmlBody.Append("Sincerely,");
            htmlBody.Append("<br/>");
            htmlBody.Append("Highway Maintenance Contract Reporting System");
            htmlBody.Append("<br/>");
            htmlBody.Append("Ministry of Transportation and Infrastructure");

            return htmlBody.ToString();
        }
    }
}
