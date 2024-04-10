using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hmcr.Data.Database.Entities
{
    public class HmrSaltReportAppendix
    {
        public decimal AppendixId { get; set; } // Primary Key
        public decimal SaltReportId { get; set; } // Foreign Key
        public HmrSaltReport SaltReport { get; set; } // Navigation property

        [Column("NEW_SALT_DOME_PAD_ID")]
        public int? NewSaltDomeWithPadIdentified { get; set; }

        [Column("NEW_SALT_DOME_PAD_ACH")]
        public int? NewSaltDomeWithPadAchieved { get; set; }

        [Column("NEW_SALT_DOME_IND_ID")]
        public int? NewSaltDomeIndoorStorageIdentified { get; set; }

        [Column("NEW_SALT_DOME_IND_ACH")]
        public int? NewSaltDomeIndoorStorageAchieved { get; set; }

        [Column("UPG_SALT_STOR_SITES_ID")]
        public int? UpgradeSaltStorageSitesIdentified { get; set; }

        [Column("UPG_SALT_STOR_SITES_ACH")]
        public int? UpgradeSaltStorageSitesAchieved { get; set; }

        [Column("CONS_PERM_COV_STRUC_ID")]
        public int? ConstructPermanentCoverStructureIdentified { get; set; }

        [Column("CONS_PERM_COV_STRUC_ACH")]
        public int? ConstructPermanentCoverStructureAchieved { get; set; }

        [Column("IMP_PAD_ABR_ID")]
        public int? ImpermeablePadForAbrasivesIdentified { get; set; }

        [Column("IMP_PAD_ABR_ACH")]
        public int? ImpermeablePadForAbrasivesAchieved { get; set; }

        [Column("EXP_INDOOR_ABR_ID")]
        public int? ExpandInsideBuildingForAbrasivesIdentified { get; set; }

        [Column("EXP_INDOOR_ABR_ACH")]
        public int? ExpandInsideBuildingForAbrasivesAchieved { get; set; }

        [Column("USE_TARPS_ABR_ID")]
        public int? UseTarpsForAbrasivesIdentified { get; set; }

        [Column("USE_TARPS_ABR_ACH")]
        public int? UseTarpsForAbrasivesAchieved { get; set; }

        [Column("RECONF_STOR_CAP_ID")]
        public int? ReconfigureStorageCapacityIdentified { get; set; }

        [Column("RECONF_STOR_CAP_ACH")]
        public int? ReconfigureStorageCapacityAchieved { get; set; }

        [Column("RECONF_OP_FAC_ID")]
        public int? ReconfigureOperationFacilitiesIdentified { get; set; }

        [Column("RECONF_OP_FAC_ACH")]
        public int? ReconfigureOperationFacilitiesAchieved { get; set; }

        [Column("DES_TRK_LOAD_AREA_ID")]
        public int? DesignAreaForTruckLoadingIdentified { get; set; }

        [Column("DES_TRK_LOAD_AREA_ACH")]
        public int? DesignAreaForTruckLoadingAchieved { get; set; }

        [Column("CTRL_TRK_LOAD_ID")]
        public int? ControlTruckLoadingIdentified { get; set; }

        [Column("CTRL_TRK_LOAD_ACH")]
        public int? ControlTruckLoadingAchieved { get; set; }

        [Column("INST_EQ_WASH_BAY_ID")]
        public int? InstallEquipmentWashBayIdentified { get; set; }

        [Column("INST_EQ_WASH_BAY_ACH")]
        public int? InstallEquipmentWashBayAchieved { get; set; }

        [Column("DES_RUNOFF_CTRL_ID")]
        public int? DesignSiteForRunoffControlIdentified { get; set; }

        [Column("DES_RUNOFF_CTRL_ACH")]
        public int? DesignSiteForRunoffControlAchieved { get; set; }

        [Column("MAN_SALT_CONT_WAT_ID")]
        public int? ManageSaltContaminatedWatersIdentified { get; set; }

        [Column("MAN_SALT_CONT_WAT_ACH")]
        public int? ManageSaltContaminatedWatersAchieved { get; set; }

        [Column("SPILL_PREV_PLAN_ID")]
        public int? SpillPreventionPlanIdentified { get; set; }

        [Column("SPILL_PREV_PLAN_ACH")]
        public int? SpillPreventionPlanAchieved { get; set; }

        [Column("REM_CONT_SNOW_ID")]
        public int? RemoveContaminatedSnowIdentified { get; set; }

        [Column("REM_CONT_SNOW_ACH")]
        public int? RemoveContaminatedSnowAchieved { get; set; }

        [Column("OTHER_SPEC_ID")]
        public int? OtherSpecifyIdentified { get; set; }

        [Column("OTHER_SPEC_ACH")]
        public int? OtherSpecifyAchieved { get; set; }

        [Column("INST_GRND_SPD_CTRL_ID")]
        public int? InstallGroundSpeedControlsIdentified { get; set; }

        [Column("INST_GRND_SPD_CTRL_ACH")]
        public int? InstallGroundSpeedControlsAchieved { get; set; }

        [Column("INC_PRE_WET_EQ_ID")]
        public int? IncreasePreWettingEquipmentIdentified { get; set; }

        [Column("INC_PRE_WET_EQ_ACH")]
        public int? IncreasePreWettingEquipmentAchieved { get; set; }

        [Column("INST_LIQ_ANTI_IC_ID")]
        public int? InstallLiquidAntiIcingIdentified { get; set; }

        [Column("INST_LIQ_ANTI_IC_ACH")]
        public int? InstallLiquidAntiIcingAchieved { get; set; }

        [Column("INST_IR_THERM_ID")]
        public int? InstallInfraredThermometersIdentified { get; set; }

        [Column("INST_IR_THERM_ACH")]
        public int? InstallInfraredThermometersAchieved { get; set; }

        [Column("INST_ADD_RWIS_ID")]
        public int? InstallAdditionalRWISStationsIdentified { get; set; }

        [Column("INST_ADD_RWIS_ACH")]
        public int? InstallAdditionalRWISStationsAchieved { get; set; }

        [Column("ACC_RWIS_DATA_ID")]
        public int? AccessRWISDataIdentified { get; set; }

        [Column("ACC_RWIS_DATA_ACH")]
        public int? AccessRWISDataAchieved { get; set; }

        [Column("INST_MOBILE_RWIS_ID")]
        public int? InstallMobileRWISIdentified { get; set; }

        [Column("INST_MOBILE_RWIS_ACH")]
        public int? InstallMobileRWISAchieved { get; set; }

        [Column("ACC_MET_SRV_ID")]
        public int? AccessMeteorologicalServiceIdentified { get; set; }

        [Column("ACC_MET_SRV_ACH")]
        public int? AccessMeteorologicalServiceAchieved { get; set; }

        [Column("ADPT_PRE_WET_NET_ID")]
        public int? AdoptPreWettingMajorityNetworkIdentified { get; set; }

        [Column("ADPT_PRE_WET_NET_ACH")]
        public int? AdoptPreWettingMajorityNetworkAchieved { get; set; }

        [Column("USE_PRE_TRT_SALT_ID")]
        public int? UsePreTreatedSaltIdentified { get; set; }

        [Column("USE_PRE_TRT_SALT_ACH")]
        public int? UsePreTreatedSaltAchieved { get; set; }

        [Column("ADPT_PRE_WET_ABR_ID")]
        public int? AdoptPreWettingOrTreatmentAbrasivesIdentified { get; set; }

        [Column("ADPT_PRE_WET_ABR_ACH")]
        public int? AdoptPreWettingOrTreatmentAbrasivesAchieved { get; set; }

        [Column("TEST_NEW_PROD_ID")]
        public int? TestingNewProductsIdentified { get; set; }

        [Column("TEST_NEW_PROD_ACH")]
        public int? TestingNewProductsAchieved { get; set; }

        [Column("ADPT_ANTI_ICING_ID")]
        public int? AdoptAntiIcingStandardIdentified { get; set; }

        [Column("ADPT_ANTI_ICING_ACH")]
        public int? AdoptAntiIcingStandardAchieved { get; set; }

        [Column("INST_GPS_COMP_SYS_ID")]
        public int? InstallGPSAndComputerSystemsIdentified { get; set; }

        [Column("INST_GPS_COMP_SYS_ACH")]
        public int? InstallGPSAndComputerSystemsAchieved { get; set; }

        [Column("USE_APP_RATE_CHART_ID")]
        public int? UseChartForApplicationRatesIdentified { get; set; }

        [Column("USE_APP_RATE_CHART_ACH")]
        public int? UseChartForApplicationRatesAchieved { get; set; }

        [Column("USE_MDSS_ID")]
        public int? UseMDSSIdentified { get; set; }

        [Column("USE_MDSS_ACH")]
        public int? UseMDSSAchieved { get; set; }

        [Column("REV_SALT_USE_ID")]
        public int? ReviewSaltUseIdentified { get; set; }

        [Column("REV_SALT_USE_ACH")]
        public int? ReviewSaltUseAchieved { get; set; }

        [Column("ASS_PLW_EFF_ID")]
        public int? AssessPlowingEfficiencyIdentified { get; set; }

        [Column("ASS_PLW_EFF_ACH")]
        public int? AssessPlowingEfficiencyAchieved { get; set; }

        [Column("OTHER_ID")]
        public int? OtherIdentified { get; set; }

        [Column("OTHER_ACH")]
        public int? OtherAchieved { get; set; }

        [Column("DEV_PROG_PHASE_OUT_ID")]
        public int? DevelopProgramPhaseOutIdentified { get; set; }

        [Column("DEV_PROG_PHASE_OUT_ACH")]
        public int? DevelopProgramPhaseOutAchieved { get; set; }

        [Column("INST_NEW_SITE_LP_ID")]
        public int? InstallNewSiteLowPermeabilityIdentified { get; set; }

        [Column("INST_NEW_SITE_LP_ACH")]
        public int? InstallNewSiteLowPermeabilityAchieved { get; set; }

        [Column("UPG_SITE_LP_ID")]
        public int? UpgradeExistingSiteLowPermeabilityIdentified { get; set; }

        [Column("UPG_SITE_LP_ACH")]
        public int? UpgradeExistingSiteLowPermeabilityAchieved { get; set; }

        [Column("COL_MELT_WATER_PT_ID")]
        public int? CollectMeltWaterSpecificPointIdentified { get; set; }

        [Column("COL_MELT_WATER_PT_ACH")]
        public int? CollectMeltWaterSpecificPointAchieved { get; set; }

        [Column("CONS_COLL_POND_ID")]
        public int? ConstructCollectionPondIdentified { get; set; }

        [Column("CONS_COLL_POND_ACH")]
        public int? ConstructCollectionPondAchieved { get; set; }

        [Column("OTHER_SNOW_DISP_ID")]
        public int? OtherSnowDisposalIdentified { get; set; }

        [Column("OTHER_SNOW_DISP_ACH")]
        public int? OtherSnowDisposalAchieved { get; set; }

        [Column("ID_SALT_VUL_AREAS_ID")]
        public int? IdentifySaltVulnerableAreasIdentified { get; set; }

        [Column("ID_SALT_VUL_AREAS_ACH")]
        public int? IdentifySaltVulnerableAreasAchieved { get; set; }

        [Column("PRIO_AREAS_ADD_PROT_ID")]
        public int? PrioritizeAreasForAdditionalProtectionIdentified { get; set; }

        [Column("PRIO_AREAS_ADD_PROT_ACH")]
        public int? PrioritizeAreasForAdditionalProtectionAchieved { get; set; }

        [Column("IMPL_PROT_MIT_MEAS_ID")]
        public int? ImplementProtectionMitigationMeasuresIdentified { get; set; }

        [Column("IMPL_PROT_MIT_MEAS_ACH")]
        public int? ImplementProtectionMitigationMeasuresAchieved { get; set; }

        [Column("CON_ENV_MON_ID")]
        public int? ConductEnvironmentalMonitoringIdentified { get; set; }

        [Column("CON_ENV_MON_ACH")]
        public int? ConductEnvironmentalMonitoringAchieved { get; set; }

        [Column("OTHER_VUL_AREAS_ID")]
        public int? OtherVulnerableAreasIdentified { get; set; }

        [Column("OTHER_VUL_AREAS_ACH")]
        public int? OtherVulnerableAreasAchieved { get; set; }

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