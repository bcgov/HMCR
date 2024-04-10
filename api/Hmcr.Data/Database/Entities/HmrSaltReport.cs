using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Hmcr.Data.Database.Entities
{
    public class HmrSaltReport
    {
        public decimal SaltReportId { get; set; }

        public decimal ServiceArea { get; set; }

        [Column("CNTCT_NAME")]
        public string ContactName { get; set; }

        [Column("TEL")]
        public string Telephone { get; set; }

        [Column("EMAIL")]
        public string Email { get; set; }

        [Column("PLAN_DEV")]
        public string PlanDeveloped { get; set; }

        [Column("PLAN_REV")]
        public string PlanReviewed { get; set; }

        [Column("PLAN_UPD")]
        public string PlanUpdated { get; set; }

        [Column("MGR_TRAIN")]
        public string ManagerTraining { get; set; }

        [Column("SUPV_TRAIN")]
        public string SupervisorTraining { get; set; }

        [Column("OPR_TRAIN")]
        public string OperatorTraining { get; set; }

        [Column("MECH_TRAIN")]
        public string MechanicalTraining { get; set; }

        [Column("PATR_TRAIN")]
        public string PatrollerTraining { get; set; }

        [Column("MAT_STOR_ID")]
        public int? MaterialStorageIdentified { get; set; }

        [Column("MAT_STOR_ACH")]
        public int? MaterialStorageAchieved { get; set; }

        [Column("SALT_APP_ID")]
        public int? SaltApplicationIdentified { get; set; }

        [Column("SALT_APP_ACH")]
        public int? SaltApplicationAchieved { get; set; }


        // Section 2
        [Column("RD_TOT_LEN")]
        public int? RoadTotalLength { get; set; }

        [Column("SALT_TOT_DAYS")]
        public int? SaltTotalDays { get; set; }


        // Section 3
        [Column("DEICER_NACL")]
        public decimal? DeicerNacl { get; set; }

        [Column("DEICER_MGCL2")]
        public decimal? DeicerMgcl2 { get; set; }

        [Column("DEICER_CACL2")]
        public decimal? DeicerCacl2 { get; set; }

        [Column("DEICER_ACET")]
        public decimal? DeicerAcetate { get; set; }


        [Column("TRTD_ABR_SDST")]
        public decimal? TreatedAbrasivesSandstoneDust { get; set; }

        [Column("TRTD_ABR_NACL")]
        public decimal? TreatedAbrasivesNacl { get; set; }

        [Column("TRTD_ABR_MGCL2")]
        public decimal? TreatedAbrasivesMgcl2 { get; set; }

        [Column("TRTD_ABR_CACL2")]
        public decimal? TreatedAbrasivesCacl2 { get; set; }


        [Column("PRWT_NACL")]
        public decimal? PrewettingNacl { get; set; }

        [Column("PRWT_MGCL2")]
        public decimal? PrewettingMgcl2 { get; set; }

        [Column("PRWT_CACL2")]
        public decimal? PrewettingCacl2 { get; set; }

        [Column("PRWT_ACET")]
        public decimal? PrewettingAcetate { get; set; }

        [Column("PRWT_NONCL")]
        public decimal? PrewettingNonchloride { get; set; }


        [Column("PRTT_NACL")]
        public decimal? PretreatmentNacl { get; set; }

        [Column("PRTT_MGCL2")]
        public decimal? PretreatmentMgcl2 { get; set; }

        [Column("PRTT_CACL2")]
        public decimal? PretreatmentCacl2 { get; set; }

        [Column("PRTT_ACET")]
        public decimal? PretreatmentAcetate { get; set; }

        [Column("PRTT_NONCL")]
        public decimal? PretreatmentNonchloride { get; set; }


        [Column("ANTIC_NACL")]
        public decimal? AntiicingNacl { get; set; }

        [Column("ANTIC_MGCL2")]
        public decimal? AntiicingMgcl2 { get; set; }

        [Column("ANTIC_CACL2")]
        public decimal? AntiicingCacl2 { get; set; }

        [Column("ANTIC_ACET")]
        public decimal? AntiicingAcetate { get; set; }

        [Column("ANTIC_NONCL")]
        public decimal? AntiicingNonchloride { get; set; }


        [Column("MULTICHL_A_LTRS")]
        public decimal? MultichlorideALitres { get; set; }

        [Column("MULTICHL_A_NACL_PCT")]
        public decimal? MultichlorideANaclPercentage { get; set; }

        [Column("MULTICHL_A_MGCL2_PCT")]
        public decimal? MultichlorideAMgcl2Percentage { get; set; }

        [Column("MULTICHL_A_CACL2_PCT")]
        public decimal? MultichlorideACacl2Percentage { get; set; }

        [Column("MULTICHL_B_LTRS")]
        public decimal? MultichlorideBLitres { get; set; }

        [Column("MULTICHL_B_NACL_PCT")]
        public decimal? MultichlorideBNaclPercentage { get; set; }

        [Column("MULTICHL_B_MGCL2_PCT")]
        public decimal? MultichlorideBMgcl2Percentage { get; set; }

        [Column("MULTICHL_B_CACL2_PCT")]
        public decimal? MultichlorideBCacl2Percentage { get; set; }


        // Section 4
        [Column("SALT_STRG_SITES_TOT")]
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

        [Column("ALL_MAT_HNDL_PLAN")]
        public bool? AllMaterialsHandledPlan { get; set; }

        [Column("ALL_MAT_HNDL_SITES")]
        public int? AllMaterialsHandledSites { get; set; }

        [Column("EQP_PRVNT_OVRLOAD_PLAN")]
        public bool? EquipmentPreventsOverloadingPlan { get; set; }

        [Column("EQP_PRVNT_OVRLOAD_SITES")]
        public int? EquipmentPreventsOverloadingSites { get; set; }

        [Column("WASTE_WATER_SYS_PLAN")]
        public bool? WastewaterSystemPlan { get; set; }

        [Column("WASTE_WATER_SYS_SITES")]
        public int? WastewaterSystemSites { get; set; }

        [Column("CTRL_DIV_EXT_WAT_PLAN")]
        public bool? ControlDiversionExternalWatersPlan { get; set; }

        [Column("CTRL_DIV_EXT_WAT_SITES")]
        public int? ControlDiversionExternalWatersSites { get; set; }

        [Column("DRAIN_COLL_SYS_PLAN")]
        public bool? DrainageCollectionSystemPlan { get; set; }

        [Column("DRAIN_COLL_SYS_SITES")]
        public int? DrainageCollectionSystemSites { get; set; }

        [Column("MUN_SEWER_SYS_PLAN")]
        public bool? MunicipalSewerSystemPlan { get; set; }

        [Column("MUN_SEWER_SYS_SITES")]
        public int? MunicipalSewerSystemSites { get; set; }

        [Column("REMOV_CONT_PLAN")]
        public bool? RemovalContainmentPlan { get; set; }

        [Column("REMOV_CONT_SITES")]
        public int? RemovalContainmentSites { get; set; }

        [Column("WATERCRS_PLAN")]
        public bool? WatercoursePlan { get; set; }

        [Column("WATERCRS_SITES")]
        public int? WatercourseSites { get; set; }

        [Column("OTH_DISCH_PT_PLAN")]
        public bool? OtherDischargePointPlan { get; set; }

        [Column("OTH_DISCH_PT_SITES")]
        public int? OtherDischargePointSites { get; set; }

        [Column("ONGOING_CLNUP_PLAN")]
        public bool? OngoingCleanupPlan { get; set; }

        [Column("ONGOING_CLNUP_SITES")]
        public int? OngoingCleanupSites { get; set; }

        [Column("RISK_MGMT_PLAN_PLAN")]
        public bool? RiskManagementPlanPlan { get; set; }

        [Column("RISK_MGMT_PLAN_SITES")]
        public int? RiskManagementPlanSites { get; set; }


        // Section 5
        [Column("NUM_VEHICLES")]
        public int NumberOfVehicles { get; set; }

        [Column("VEH_SALT_APP")]
        public int VehiclesForSaltApplication { get; set; }

        [Column("VEH_CONV")]
        public int VehiclesWithConveyors { get; set; }

        [Column("VEH_PREWET_EQ")]
        public int VehiclesWithPreWettingEquipment { get; set; }

        [Column("VEH_DLA")]
        public int VehiclesForDLA { get; set; }

        [Column("REG_CALIB")]
        public bool? RegularCalibration { get; set; }

        [Column("REG_CALIB_TOT")]
        public int? RegularCalibrationTotal { get; set; }

        [Column("IR_THRM_REL")]
        public bool? InfraredThermometerRelied { get; set; }

        [Column("IR_THRM_TOT")]
        public int InfraredThermometerTotal { get; set; }

        [Column("MET_SVC_REL")]
        public bool? MeteorologicalServiceRelied { get; set; }

        [Column("FIX_RWIS_REL")]
        public bool? FixedRWISStationsRelied { get; set; }

        [Column("FIX_RWIS_TOT")]
        public int FixedRWISStationsTotal { get; set; }

        [Column("MOB_RWIS_REL")]
        public bool? MobileRWISMountedRelied { get; set; }

        [Column("MOB_RWIS_TOT")]
        public int MobileRWISMountedTotal { get; set; }

        [Column("AVL_REL")]
        public bool? AVLRelied { get; set; }

        [Column("AVL_TOT")]
        public int AVLTotal { get; set; }

        [Column("SALT_APP_RATE_REL")]
        public bool? SaltApplicationRatesRelied { get; set; }

        [Column("SALT_APP_RATE_TOT")]
        public int SaltApplicationRatesTotal { get; set; }

        [Column("APP_RATE_CHRT_REL")]
        public bool? ApplicationRateChartRelied { get; set; }

        [Column("APP_RATE_CHRT_TOT")]
        public int ApplicationRateChartTotal { get; set; }

        [Column("TEST_MDSS_REL")]
        public bool? TestingMDSSRelied { get; set; }

        [Column("TEST_SMDS_TOT")]
        public int TestingSMDSTotal { get; set; }


        // Section 6
        [Column("SNOW_DISP_SITE_USED")]
        public bool? SnowDisposalSiteUsed { get; set; }

        [Column("SNOW_DISP_SITE_TOT")]
        public int? SnowDisposalSiteTotal { get; set; }

        [Column("SNOW_MELT_USED")]
        public bool? SnowMeltersUsed { get; set; }

        [Column("MELTWATER_DISP_METH_USED")]
        public bool? MeltwaterDisposalMethodUsed { get; set; }


        // Section 7
        [Column("COMP_INV")]
        public string CompletedInventory { get; set; }

        [Column("SET_VULN_AREAS")]
        public string SetVulnerableAreas { get; set; }

        [Column("ACT_PLAN_PREP")]
        public string ActionPlanPrepared { get; set; }

        [Column("PROT_MEAS_IMPL")]
        public string ProtectionMeasuresImplemented { get; set; }

        [Column("ENV_MON_COND")]
        public string EnvironmentalMonitoringConducted { get; set; }

        [Column("DRINK_WATER_AREA_ID")]
        public int? DrinkingWaterAreasIdentified { get; set; }

        [Column("DRINK_WATER_PROT_ID")]
        public int? DrinkingWaterAreasWithProtection { get; set; }

        [Column("DRINK_WATER_CHLOR_ID")]
        public int? DrinkingWaterAreasWithChloride { get; set; }

        [Column("AQUA_LIFE_AREA_ID")]
        public int? AquaticLifeAreasIdentified { get; set; }

        [Column("AQUA_LIFE_PROT_ID")]
        public int? AquaticLifeAreasWithProtection { get; set; }

        [Column("AQUA_LIFE_CHLOR_ID")]
        public int? AquaticLifeAreasWithChloride { get; set; }

        [Column("WETLANDS_AREA_ID")]
        public int? WetlandsAreasIdentified { get; set; }

        [Column("WETLANDS_PROT_ID")]
        public int? WetlandsAreasWithProtection { get; set; }

        [Column("WETLANDS_CHLOR_ID")]
        public int? WetlandsAreasWithChloride { get; set; }

        [Column("DELIM_AREA_ID")]
        public int? DelimitedAreasAreasIdentified { get; set; }

        [Column("DELIM_AREA_PROT_ID")]
        public int? DelimitedAreasAreasWithProtection { get; set; }

        [Column("DELIM_AREA_CHLOR_ID")]
        public int? DelimitedAreasAreasWithChloride { get; set; }


        [Column("VAL_LANDS_AREA_ID")]
        public int? ValuedLandsAreasIdentified { get; set; }

        [Column("VAL_LANDS_PROT_ID")]
        public int? ValuedLandsAreasWithProtection { get; set; }

        [Column("VAL_LANDS_CHLOR_ID")]
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
