using System;

namespace Hmcr.Model.Dtos.FeedbackMessage
{
    public class FeedbackMessageUpdateDto
    {
        public decimal FeedbackMessageId { get; set; }
        public decimal SubmissionObjectId { get; set; }
        public string CommunicationSubject { get; set; }
        public string CommunicationText { get; set; }
        public DateTime? CommunicationDate { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsError { get; set; }
        public string SendErrorText { get; set; }
    }
}
