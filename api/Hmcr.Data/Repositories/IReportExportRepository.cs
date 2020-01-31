using Hmcr.Model.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IReportExportRepository<T> where T : IReportExportDto
    {
        Task<IEnumerable<T>> ExporReportAsync(decimal submissionObjectId);
    }
}
