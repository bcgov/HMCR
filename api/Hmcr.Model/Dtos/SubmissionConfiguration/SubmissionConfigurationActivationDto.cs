using System.ComponentModel.DataAnnotations;

namespace Hmcr.Model.Dtos.SubmissionConfiguration
{
    public class SubmissionConfigurationActivationDto
    {
        [Required]
        public bool? IsActive { get; set; }

        [Required]
        public long? ConcurrencyControlNumber { get; set; }
    }
}
