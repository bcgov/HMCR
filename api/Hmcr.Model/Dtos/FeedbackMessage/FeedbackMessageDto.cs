using System;

namespace Hmcr.Model.Dtos.FeedbackMessage
{
    public class FeedbackMessageDto
    {
        public decimal SubmissionObjectId { get; set; }
        public string CommunicationSubject { get; set; }
        public string CommunicationText { get; set; }
        public DateTime? CommunicationDate { get; set; }
        public string IsSent { get; set; }
        public string IsError { get; set; }
        public string SendErrorText { get; set; }
    }
}
