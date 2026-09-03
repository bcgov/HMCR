using System.Collections.Generic;

namespace Hmcr.Model.Dtos.NotificationPreference
{
    public class NotificationPreferencesDto
    {
        public NotificationPreferencesDto()
        {
            ServiceAreas = new List<NotificationPreferenceServiceAreaDto>();
        }

        public IList<NotificationPreferenceServiceAreaDto> ServiceAreas { get; set; }
    }

    public class NotificationPreferenceServiceAreaDto
    {
        public NotificationPreferenceServiceAreaDto()
        {
            ReportTypes = new List<NotificationPreferenceReportTypeDto>();
        }

        public decimal ServiceAreaNumber { get; set; }
        public string ServiceAreaName { get; set; }
        public IList<NotificationPreferenceReportTypeDto> ReportTypes { get; set; }
    }

    public class NotificationPreferenceReportTypeDto
    {
        public decimal SubmissionStreamId { get; set; }
        public string ReportTypeName { get; set; }
        public string StagingTableName { get; set; }
        public bool SuccessfulUploadsEnabled { get; set; }
        public bool UploadErrorsEnabled { get; set; }
    }
}
