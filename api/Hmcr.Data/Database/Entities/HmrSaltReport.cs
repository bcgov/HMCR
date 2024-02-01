using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hmcr.Data.Database.Entities
{
    public class HmrSaltReport
    {
        public int SaltReportId { get; set; }
        public int ServiceArea { get; set; }
        public string ContactName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string PlanDeveloped { get; set; }
        public string PlanReviewed { get; set; }
        public string PlanUpdated { get; set; }
        public string ManagerTraining { get; set; }
        public string SupervisorTraining { get; set; }
        public string OperatorTraining { get; set; }
        public string MechanicalTraining { get; set; }
        public string PatrollerTraining { get; set; }
        public int? MaterialStorageIdentified { get; set; }
        public int? MaterialStorageAchieved { get; set; }
        public int? SaltApplicationIdentified { get; set; }
        public int? SaltApplicationAchieved { get; set; }

        // Section 2
        public int RoadTotalLength { get; set; }
        public int SaltTotalDays { get; set; }

        // Section 3
        public decimal? DeicerNacl { get; set; }
        public decimal? DeicerMgcl2 { get; set; }
        public decimal? DeicerCacl2 { get; set; }
        public decimal? DeicerAcetate { get; set; }

        public decimal? TreatedAbrasivesSandstoneDust { get; set; }
        public decimal? TreatedAbrasivesNacl { get; set; }
        public decimal? TreatedAbrasivesMgcl2 { get; set; }
        public decimal? TreatedAbrasivesCacl2 { get; set; }

        public decimal? PrewettingNacl { get; set; }
        public decimal? PrewettingMgcl2 { get; set; }
        public decimal? PrewettingCacl2 { get; set; }
        public decimal? PrewettingAcetate { get; set; }
        public decimal? PrewettingNonchloride { get; set; }

        public decimal? PretreatmentNacl { get; set; }
        public decimal? PretreatmentMgcl2 { get; set; }
        public decimal? PretreatmentCacl2 { get; set; }
        public decimal? PretreatmentAcetate { get; set; }
        public decimal? PretreatmentNonchloride { get; set; }

        public decimal? AntiicingNacl { get; set; }
        public decimal? AntiicingMgcl2 { get; set; }
        public decimal? AntiicingCacl2 { get; set; }
        public decimal? AntiicingAcetate { get; set; }
        public decimal? AntiicingNonchloride { get; set; }

        public decimal? MultichlorideALitres { get; set; }
        public decimal? MultichlorideANaclPercentage { get; set; }
        public decimal? MultichlorideAMgcl2Percentage { get; set; }
        public decimal? MultichlorideACacl2Percentage { get; set; }
        public decimal? MultichlorideBLitres { get; set; }
        public decimal? MultichlorideBNaclPercentage { get; set; }
        public decimal? MultichlorideBMgcl2Percentage { get; set; }
        public decimal? MultichlorideBCacl2Percentage { get; set; }

        // Section 4
        public int? SaltStorageSitesTotal { get; set; }
        public ICollection<HmrSaltStockpile> Stockpiles { get; set; }
        public int RoadSaltStockpilesTotal => Stockpiles?.Sum(x => x.RoadSaltStockpilesTotal) ?? 0;
        public int RoadSaltOnImpermeableSurface => Stockpiles?.Sum(x => x.RoadSaltOnImpermeableSurface) ?? 0;
        public int RoadSaltUnderPermanentRoof => Stockpiles?.Sum(x => x.RoadSaltUnderPermanentRoof) ?? 0;
        public int RoadSaltUnderTarp => Stockpiles?.Sum(x => x.RoadSaltUnderTarp) ?? 0;
        public int TreatedAbrasivesStockpilesTotal => Stockpiles?.Sum(x => x.TreatedAbrasivesStockpilesTotal) ?? 0;
        public int TreatedAbrasivesOnImpermeableSurface => Stockpiles?.Sum(x => x.TreatedAbrasivesOnImpermeableSurface) ?? 0;
        public int TreatedAbrasivesUnderPermanentRoof => Stockpiles?.Sum(x => x.TreatedAbrasivesUnderPermanentRoof) ?? 0;
        public int TreatedAbrasivesUnderTarp => Stockpiles?.Sum(x => x.TreatedAbrasivesUnderTarp) ?? 0;
        public bool? AllMaterialsHandledPlan { get; set; }
        public int? AllMaterialsHandledSites { get; set; }
        public bool? EquipmentPreventsOverloadingPlan { get; set; }
        public int? EquipmentPreventsOverloadingSites { get; set; }
        public bool? WastewaterSystemPlan { get; set; }
        public int? WastewaterSystemSites { get; set; }
        public bool? ControlDiversionExternalWatersPlan { get; set; }
        public int? ControlDiversionExternalWatersSites { get; set; }
        public bool? DrainageCollectionSystemPlan { get; set; }
        public int? DrainageCollectionSystemSites { get; set; }
        public bool? MunicipalSewerSystemPlan { get; set; }
        public int? MunicipalSewerSystemSites { get; set; }
        public bool? RemovalContainmentPlan { get; set; }
        public int? RemovalContainmentSites { get; set; }
        public bool? WatercoursePlan { get; set; }
        public int? WatercourseSites { get; set; }
        public bool? OtherDischargePointPlan { get; set; }
        public int? OtherDischargePointSites { get; set; }
        public bool? OngoingCleanupPlan { get; set; }
        public int? OngoingCleanupSites { get; set; }
        public bool? RiskManagementPlanPlan { get; set; }
        public int? RiskManagementPlanSites { get; set; }

        // Section 5
        public int NumberOfVehicles { get; set; }
        public int VehiclesForSaltApplication { get; set; }
        public int VehiclesWithConveyors { get; set; }
        public int VehiclesWithPreWettingEquipment { get; set; }
        public int VehiclesForDLA { get; set; }
        public bool? RegularCalibration { get; set; }
        public int? RegularCalibrationTotal { get; set; }
        public bool? InfraredThermometerRelied { get; set; }
        public int InfraredThermometerTotal { get; set; }
        public bool? MeteorologicalServiceRelied { get; set; }
        public bool? FixedRWISStationsRelied { get; set; }
        public int FixedRWISStationsTotal { get; set; }
        public bool? MobileRWISMountedRelied { get; set; }
        public int MobileRWISMountedTotal { get; set; }
        public bool? AVLRelied { get; set; }
        public int AVLTotal { get; set; }
        public bool? SaltApplicationRatesRelied { get; set; }
        public int SaltApplicationRatesTotal { get; set; }
        public bool? ApplicationRateChartRelied { get; set; }
        public int ApplicationRateChartTotal { get; set; }
        public bool? TestingMDSSRelied { get; set; }
        public int TestingSMDSTotal { get; set; }

        // Section 6
        public bool? SnowDisposalSiteUsed { get; set; }
        public int? SnowDisposalSiteTotal { get; set; }
        public bool? SnowMeltersUsed { get; set; }
        public bool? MeltwaterDisposalMethodUsed { get; set; }

        // Section 7
        public string CompletedInventory { get; set; }
        public string SetVulnerableAreas { get; set; }
        public string ActionPlanPrepared { get; set; }
        public string ProtectionMeasuresImplemented { get; set; }
        public string EnvironmentalMonitoringConducted { get; set; }
        public int? DrinkingWaterAreasIdentified { get; set; }
        public int? DrinkingWaterAreasWithProtection { get; set; }
        public int? DrinkingWaterAreasWithChloride { get; set; }

        public int? AquaticLifeAreasIdentified { get; set; }
        public int? AquaticLifeAreasWithProtection { get; set; }
        public int? AquaticLifeAreasWithChloride { get; set; }

        public int? WetlandsAreasIdentified { get; set; }
        public int? WetlandsAreasWithProtection { get; set; }
        public int? WetlandsAreasWithChloride { get; set; }

        public int? DelimitedAreasAreasIdentified { get; set; }
        public int? DelimitedAreasAreasWithProtection { get; set; }
        public int? DelimitedAreasAreasWithChloride { get; set; }

        public int? ValuedLandsAreasIdentified { get; set; }
        public int? ValuedLandsAreasWithProtection { get; set; }
        public int? ValuedLandsAreasWithChloride { get; set; }

        // Appendix
        public HmrSaltReportAppendix Appendix { get; set; }

        // Default
        public long ConcurrencyControlNumber { get; set; }
        public string AppCreateUserid { get; set; }
        public DateTime AppCreateTimestamp { get; set; }
        public Guid AppCreateUserGuid { get; set; }
        public string AppCreateUserDirectory { get; set; }
        public string AppLastUpdateUserid { get; set; }
        public DateTime AppLastUpdateTimestamp { get; set; }
        public Guid AppLastUpdateUserGuid { get; set; }
        public string AppLastUpdateUserDirectory { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime? DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime? DbAuditLastUpdateTimestamp { get; set; }
    }
}
