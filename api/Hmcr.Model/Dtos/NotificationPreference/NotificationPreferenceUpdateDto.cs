using System.ComponentModel.DataAnnotations;

namespace Hmcr.Model.Dtos.NotificationPreference
{
    public class NotificationPreferenceUpdateDto
    {
        [Required]
        public bool? SuccessfulUploadsEnabled { get; set; }

        [Required]
        public bool? UploadErrorsEnabled { get; set; }
    }

    public class NotificationPreferenceBulkUpdateDto
    {
        [Required]
        public bool? Enabled { get; set; }
    }
}
