using Microsoft.AspNetCore.Http;

namespace Hmcr.Model.Dtos.WorkReport
{
    public class WorkRptUploadDto
    {
        public decimal ServiceAreaNumber { get; set; }
        public IFormFile ReportFile { get; set; }
    }
}
