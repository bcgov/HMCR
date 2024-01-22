using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.SaltReport;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
            var stockPileEntity = dto.Sect4.Stockpiles?.Select(MapStockpileDtoToEntity).ToList();

            // TODO: ADD ADDITIONAL BUSINESS LOGIC
            // CODE HERE...
            // ProcessBusinessRules(saltReport)

            return await _repository.AddAsync(saltReportEntity, stockPileEntity);
        }

        private static HmrSaltReport MapDtoToEntity(SaltReportDto dto)
        {
            var entity = new HmrSaltReport
            {
                // Section 1
                ServiceArea = dto.ServiceArea,
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
                DeicerNacl = dto.Sect3.Deicer.Nacl,
                DeicerMgcl2 = dto.Sect3.Deicer.Mgcl2,
                DeicerCacl2 = dto.Sect3.Deicer.Cacl2,
                DeicerAcetate = dto.Sect3.Deicer.Acetate,
                TreatedAbrasivesSandstoneDust = dto.Sect3.TreatedAbrasives.SandstoneDust,
                TreatedAbrasivesNacl = dto.Sect3.TreatedAbrasives.Nacl,
                TreatedAbrasivesMgcl2 = dto.Sect3.TreatedAbrasives.Mgcl2,
                TreatedAbrasivesCacl2 = dto.Sect3.TreatedAbrasives.Cacl2,
                PrewettingNacl = dto.Sect3.Prewetting.Nacl,
                PrewettingMgcl2 = dto.Sect3.Prewetting.Mgcl2,
                PrewettingCacl2 = dto.Sect3.Prewetting.Cacl2,
                PrewettingAcetate = dto.Sect3.Prewetting.Acetate,
                PrewettingNonchloride = dto.Sect3.Prewetting.Nonchloride,
                PretreatmentNacl = dto.Sect3.Pretreatment.Nacl,
                PretreatmentMgcl2 = dto.Sect3.Pretreatment.Mgcl2,
                PretreatmentCacl2 = dto.Sect3.Pretreatment.Cacl2,
                PretreatmentAcetate = dto.Sect3.Pretreatment.Acetate,
                PretreatmentNonchloride = dto.Sect3.Pretreatment.Nonchloride,
                AntiicingNacl = dto.Sect3.Antiicing.Nacl,
                AntiicingMgcl2 = dto.Sect3.Antiicing.Mgcl2,
                AntiicingCacl2 = dto.Sect3.Antiicing.Cacl2,
                AntiicingAcetate = dto.Sect3.Antiicing.Acetate,
                AntiicingNonchloride = dto.Sect3.Antiicing.Nonchloride,
                MultichlorideALitres = dto.Sect3.MultiChlorideA.Litres,
                MultichlorideANaclPercentage = dto.Sect3.MultiChlorideA.NaclPercentage,
                MultichlorideAMgcl2Percentage = dto.Sect3.MultiChlorideA.Mgcl2Percentage,
                MultichlorideACacl2Percentage = dto.Sect3.MultiChlorideA.Cacl2Percentage,
                MultichlorideBLitres = dto.Sect3.MultiChlorideB.Litres,
                MultichlorideBNaclPercentage = dto.Sect3.MultiChlorideB.NaclPercentage,
                MultichlorideBMgcl2Percentage = dto.Sect3.MultiChlorideB.Mgcl2Percentage,
                MultichlorideBCacl2Percentage = dto.Sect3.MultiChlorideB.Cacl2Percentage,

                // Section 4
                SaltStorageSitesTotal = dto.Sect4.SaltStorageSitesTotal,
                AllMaterialsHandledPlan = dto.Sect4.Practices.AllMaterialsHandled.HasPlan,
                AllMaterialsHandledSites = dto.Sect4.Practices.AllMaterialsHandled.NumSites,
                EquipmentPreventsOverloadingPlan = dto.Sect4.Practices.EquipmentPreventsOverloading.HasPlan,
                EquipmentPreventsOverloadingSites = dto.Sect4.Practices.EquipmentPreventsOverloading.NumSites,
                WastewaterSystemPlan = dto.Sect4.Practices.WastewaterSystem.HasPlan,
                WastewaterSystemSites = dto.Sect4.Practices.WastewaterSystem.NumSites,
                ControlDiversionExternalWatersPlan = dto.Sect4.Practices.ControlDiversionExternalWaters.HasPlan,
                ControlDiversionExternalWatersSites = dto.Sect4.Practices.ControlDiversionExternalWaters.NumSites,
                DrainageCollectionSystemPlan = dto.Sect4.Practices.DrainageCollectionSystem.HasPlan,
                DrainageCollectionSystemSites = dto.Sect4.Practices.DrainageCollectionSystem.NumSites,
                OngoingCleanupPlan = dto.Sect4.Practices.OngoingCleanup.HasPlan,
                OngoingCleanupSites = dto.Sect4.Practices.OngoingCleanup.NumSites,
                RiskManagementPlanPlan = dto.Sect4.Practices.RiskManagementPlan.HasPlan,
                RiskManagementPlanSites = dto.Sect4.Practices.RiskManagementPlan.NumSites,
                MunicipalSewerSystemPlan = dto.Sect4.Practices.MunicipalSewerSystem.HasPlan,
                MunicipalSewerSystemSites = dto.Sect4.Practices.MunicipalSewerSystem.NumSites,
                RemovalContainmentPlan = dto.Sect4.Practices.RemovalContainment.HasPlan,
                RemovalContainmentSites = dto.Sect4.Practices.RemovalContainment.NumSites,
                WatercoursePlan = dto.Sect4.Practices.Watercourse.HasPlan,
                WatercourseSites = dto.Sect4.Practices.Watercourse.NumSites,
                OtherDischargePointPlan = dto.Sect4.Practices.OtherDischargePoint.HasPlan,
                OtherDischargePointSites = dto.Sect4.Practices.OtherDischargePoint.NumSites,


                // Section 5
                NumberOfVehicles = dto.Sect5.NumberOfVehicles,
                VehiclesForSaltApplication = dto.Sect5.VehiclesForSaltApplication,
                VehiclesWithConveyors = dto.Sect5.VehiclesWithConveyors,
                VehiclesWithPreWettingEquipment = dto.Sect5.VehiclesWithPreWettingEquipment,
                VehiclesForDLA = dto.Sect5.VehiclesForDLA,
                InfraredThermometerRelied = dto.Sect5.WeatherMonitoringSources.InfraredThermometer.Relied,
                MeteorologicalServiceRelied = dto.Sect5.WeatherMonitoringSources.MeteorologicalService.Relied,
                FixedRWISStationsRelied = dto.Sect5.WeatherMonitoringSources.FixedRWISStations.Relied,
                MobileRWISMountedRelied = dto.Sect5.WeatherMonitoringSources.MobileRWISMounted.Relied,
                InfraredThermometerTotal = dto.Sect5.WeatherMonitoringSources.InfraredThermometer.Number,
                FixedRWISStationsTotal = dto.Sect5.WeatherMonitoringSources.FixedRWISStations.Number,
                MobileRWISMountedTotal = dto.Sect5.WeatherMonitoringSources.MobileRWISMounted.Number,
                AVLRelied = dto.Sect5.MaintenanceDecisionSupport.AVL.Relied,
                SaltApplicationRatesRelied = dto.Sect5.MaintenanceDecisionSupport.SaltApplicationRates.Relied,
                ApplicationRateChartRelied = dto.Sect5.MaintenanceDecisionSupport.ApplicationRateChart.Relied,
                TestingMDSSRelied = dto.Sect5.MaintenanceDecisionSupport.TestingMDSS.Relied,
                AVLTotal = dto.Sect5.MaintenanceDecisionSupport.AVL.Number,
                SaltApplicationRatesTotal = dto.Sect5.MaintenanceDecisionSupport.SaltApplicationRates.Number,
                ApplicationRateChartTotal = dto.Sect5.MaintenanceDecisionSupport.ApplicationRateChart.Number,
                TestingSMDSTotal = dto.Sect5.MaintenanceDecisionSupport.TestingMDSS.Number,

                // Section 6
                SnowDisposalSiteUsed = dto.Sect6.Disposal.Used,
                SnowMeltersUsed = dto.Sect6.SnowMelter.Used,
                MeltwaterDisposalMethodUsed = dto.Sect6.Meltwater.Used,
                SnowDisposalSiteTotal = dto.Sect6.Disposal.Total,
            };

            return entity;
        }

        private HmrSaltStockpile MapStockpileDtoToEntity(StockpileDto dto)
        {
            return new HmrSaltStockpile
            {
                SiteName = dto.SiteName,
                MotiOwned = dto.MotiOwned,
                RoadSaltStockpiles = dto.RoadSalts.Stockpiles,
                RoadSaltOnImpermeableSurface = dto.RoadSalts.OnImpermeableSurface,
                RoadSaltUnderPermanentRoof = dto.RoadSalts.UnderPermanentRoof,
                RoadSaltUnderTarp = dto.RoadSalts.UnderTarp,
                TreatedAbrasivesStockpiles = dto.TreatedAbrasives.Stockpiles,
                TreatedAbrasivesOnImpermeableSurface = dto.TreatedAbrasives.OnImpermeableSurface,
                TreatedAbrasivesUnderPermanentRoof = dto.TreatedAbrasives.UnderPermanentRoof,
                TreatedAbrasivesUnderTarp = dto.TreatedAbrasives.UnderTarp,
            };
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