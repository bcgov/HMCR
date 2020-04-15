using Microsoft.AspNetCore.Http;

namespace Hmcr.Model.Dtos.SubmissionObject
{
    public class FileUploadDto
    {
        public decimal ServiceAreaNumber { get; set; }
        public IFormFile ReportFile { get; set; }
    }
}
