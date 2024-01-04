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
            // TODO: DTO VALIDATION
            // CODE HERE...
            // ValidateDto(dto)

            var saltReportEntity = MapDtoToEntity(dto);

            // TODO: ADD ADDITIONAL BUSINESS LOGIC
            // CODE HERE...
            // ProcessBusinessRules(saltReport)

            return await _repository.AddAsync(saltReportEntity);
        }

        private HmrSaltReport MapDtoToEntity(SaltReportDto dto)
        {
            var entity = new HmrSaltReport
            {
                // Section 1
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

                // Section 2
                RoadTotalLength = dto.Sect2.RoadTotalLength,
                SaltTotalDays = dto.Sect2.SaltTotalDays,

                // Section 3
                DeicerNacl = dto.Sect3.DeicerNacl,
                DeicerMgcl2 = dto.Sect3.DeicerMgcl2,
                DeicerCacl2 = dto.Sect3.DeicerCacl2,
                DeicerAcetate = dto.Sect3.DeicerAcetate,
                TreatedAbrasivesSandstoneDust = dto.Sect3.TreatedAbrasivesSandstoneDust,
                TreatedAbrasivesNacl = dto.Sect3.TreatedAbrasivesNacl,
                TreatedAbrasivesMgcl2 = dto.Sect3.TreatedAbrasivesMgcl2,
                TreatedAbrasivesCacl2 = dto.Sect3.TreatedAbrasivesCacl2,
                PrewettingNacl = dto.Sect3.PrewettingNacl,
                PrewettingMgcl2 = dto.Sect3.PrewettingMgcl2,
                PrewettingCacl2 = dto.Sect3.PrewettingCacl2,
                PrewettingAcetate = dto.Sect3.PrewettingAcetate,
                PrewettingNonchloride = dto.Sect3.PrewettingNonchloride,
                PretreatmentNacl = dto.Sect3.PretreatmentNacl,
                PretreatmentMgcl2 = dto.Sect3.PretreatmentMgcl2,
                PretreatmentCacl2 = dto.Sect3.PretreatmentCacl2,
                PretreatmentAcetate = dto.Sect3.PretreatmentAcetate,
                PretreatmentNonchloride = dto.Sect3.PretreatmentNonchloride,
                AntiicingNacl = dto.Sect3.AntiicingNacl,
                AntiicingMgcl2 = dto.Sect3.AntiicingMgcl2,
                AntiicingCacl2 = dto.Sect3.AntiicingCacl2,
                AntiicingAcetate = dto.Sect3.AntiicingAcetate,
                AntiicingNonchloride = dto.Sect3.AntiicingNonchloride,
                MultichlorideALitres = dto.Sect3.MultichlorideALitres,
                MultichlorideANaclPercentage = dto.Sect3.MultichlorideANaclPercentage,
                MultichlorideAMgcl2Percentage = dto.Sect3.MultichlorideAMgcl2Percentage,
                MultichlorideACacl2Percentage = dto.Sect3.MultichlorideACacl2Percentage,
                MultichlorideBLitres = dto.Sect3.MultichlorideBLitres,
                MultichlorideBNaclPercentage = dto.Sect3.MultichlorideBNaclPercentage,
                MultichlorideBMgcl2Percentage = dto.Sect3.MultichlorideBMgcl2Percentage,
                MultichlorideBCacl2Percentage = dto.Sect3.MultichlorideBCacl2Percentage,

                // Section 5
                NumberOfVehicles = dto.Sect5.NumberOfVehicles,
                VehiclesForSaltApplication = dto.Sect5.VehiclesForSaltApplication,
                VehiclesWithConveyors = dto.Sect5.VehiclesWithConveyors,
                VehiclesWithPreWettingEquipment = dto.Sect5.VehiclesWithPreWettingEquipment,
                VehiclesForDLA = dto.Sect5.VehiclesForDLA,
                InfraredThermometerRelied = dto.Sect5.InfraredThermometerRelied,
                MeteorologicalServiceRelied = dto.Sect5.MeteorologicalServiceRelied,
                FixedRWISStationsRelied = dto.Sect5.FixedRWISStationsRelied,
                MobileRWISMountedRelied = dto.Sect5.MobileRWISMountedRelied,
                InfraredThermometerTotal = dto.Sect5.InfraredThermometerTotal,
                MeteorologicalServiceTotal = dto.Sect5.MeteorologicalServiceTotal,
                FixedRWISStationsTotal = dto.Sect5.FixedRWISStationsTotal,
                MobileRWISMountedTotal = dto.Sect5.MobileRWISMountedTotal,
                AVLRelied = dto.Sect5.AVLRelied,
                SaltApplicationRatesRelied = dto.Sect5.SaltApplicationRatesRelied,
                ApplicationRateChartRelied = dto.Sect5.ApplicationRateChartRelied,
                TestingMDSSRelied = dto.Sect5.TestingMDSSRelied,
                AVLTotal = dto.Sect5.AVLTotal,
                SaltApplicationRatesTotal = dto.Sect5.SaltApplicationRatesTotal,
                ApplicationRateChartTotal = dto.Sect5.ApplicationRateChartTotal,
                TestingSMDSTotal = dto.Sect5.TestingSMDSTotal,

                // Section 6
                SnowDisposalSiteUsed = dto.Sect6.SnowDisposalSiteUsed,
                SnowMeltersUsed = dto.Sect6.SnowMeltersUsed,
                MeltwaterDisposalMethodUsed = dto.Sect6.MeltwaterDisposalMethodUsed,
                SnowDisposalSiteTotal = dto.Sect6.SnowDisposalSiteTotal,
            };

            return entity;
        }

        public void ValidateDto(SaltReportDto dto)
        {
            // Validation Logic
        }

        public void ProcessBusinessRules(HmrSaltReport saltReport)
        {
            // Business Logic
        }
    }
}