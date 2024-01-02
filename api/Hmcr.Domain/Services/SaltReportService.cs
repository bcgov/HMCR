using System.Threading.Tasks;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.SaltReport;
using Newtonsoft.Json.Linq;

namespace Hmcr.Domain.Services
{
    public interface ISaltReportService
    {
        Task<HmrSaltReport> CreateSaltReportAsync(SaltReportDto dto);
    }

    public class SaltReportService : ISaltReportService
    {
        private readonly ISaltReportRepository _repository;

        public SaltReportService(ISaltReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<HmrSaltReport> CreateSaltReportAsync(SaltReportDto dto)
        {
            var saltReport = new HmrSaltReport
            {
                SaltReportId = 2,
                ContactName = dto.ContactName,
                Telephone = dto.Telephone,
                Email = dto.Email,
                PlanDeveloped = dto.Sect1.PlanDeveloped,
                PlanReviewed = dto.Sect1.PlanReviewed,
                PlanUpdated = dto.Sect1.PlanUpdated,
                ManagerTraining = dto.Sect1.Training.Manager,
                SupervisorTraining = dto.Sect1.Training.Supervisor,
                OperatorTraining = dto.Sect1.Training.Operator,
                MechanicalTraining = dto.Sect1.Training.Mechanical,
                PatrollerTraining = dto.Sect1.Training.Patroller,
                MaterialStorageAchieved = dto.Sect1.Objectives.MaterialStorage.Achieved,
                MaterialStorageIdentified = dto.Sect1.Objectives.MaterialStorage.Identified,
                SaltApplicationAchieved = dto.Sect1.Objectives.SaltApplication.Achieved,
                SaltApplicationIdentified = dto.Sect1.Objectives.SaltApplication.Identified,
            };

            return await _repository.AddAsync(saltReport);
        }
    }
}