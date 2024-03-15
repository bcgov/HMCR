using System;

namespace Hmcr.Data.Database.Entities
{
    public class HmrSaltReportAppendix
    {
        public int AppendixId { get; set; } // Primary Key
        public int SaltReportId { get; set; } // Foreign Key
        public HmrSaltReport SaltReport { get; set; } // Navigation property
        public int? NewSaltDomeWithPadIdentified { get; set; }
        public int? NewSaltDomeWithPadAchieved { get; set; }
        public int? NewSaltDomeIndoorStorageIdentified { get; set; }
        public int? NewSaltDomeIndoorStorageAchieved { get; set; }
        public int? UpgradeSaltStorageSitesIdentified { get; set; }
        public int? UpgradeSaltStorageSitesAchieved { get; set; }
        public int? ConstructPermanentCoverStructureIdentified { get; set; }
        public int? ConstructPermanentCoverStructureAchieved { get; set; }
        public int? ImpermeablePadForAbrasivesIdentified { get; set; }
        public int? ImpermeablePadForAbrasivesAchieved { get; set; }
        public int? ExpandInsideBuildingForAbrasivesIdentified { get; set; }
        public int? ExpandInsideBuildingForAbrasivesAchieved { get; set; }
        public int? UseTarpsForAbrasivesIdentified { get; set; }
        public int? UseTarpsForAbrasivesAchieved { get; set; }
        public int? ReconfigureStorageCapacityIdentified { get; set; }
        public int? ReconfigureStorageCapacityAchieved { get; set; }
        public int? ReconfigureOperationFacilitiesIdentified { get; set; }
        public int? ReconfigureOperationFacilitiesAchieved { get; set; }
        public int? DesignAreaForTruckLoadingIdentified { get; set; }
        public int? DesignAreaForTruckLoadingAchieved { get; set; }
        public int? ControlTruckLoadingIdentified { get; set; }
        public int? ControlTruckLoadingAchieved { get; set; }
        public int? InstallEquipmentWashBayIdentified { get; set; }
        public int? InstallEquipmentWashBayAchieved { get; set; }
        public int? DesignSiteForRunoffControlIdentified { get; set; }
        public int? DesignSiteForRunoffControlAchieved { get; set; }
        public int? ManageSaltContaminatedWatersIdentified { get; set; }
        public int? ManageSaltContaminatedWatersAchieved { get; set; }
        public int? SpillPreventionPlanIdentified { get; set; }
        public int? SpillPreventionPlanAchieved { get; set; }
        public int? RemoveContaminatedSnowIdentified { get; set; }
        public int? RemoveContaminatedSnowAchieved { get; set; }
        public int? OtherSpecifyIdentified { get; set; }
        public int? OtherSpecifyAchieved { get; set; }
        public int? InstallGroundSpeedControlsIdentified { get; set; }
        public int? InstallGroundSpeedControlsAchieved { get; set; }
        public int? IncreasePreWettingEquipmentIdentified { get; set; }
        public int? IncreasePreWettingEquipmentAchieved { get; set; }
        public int? InstallLiquidAntiIcingIdentified { get; set; }
        public int? InstallLiquidAntiIcingAchieved { get; set; }
        public int? InstallInfraredThermometersIdentified { get; set; }
        public int? InstallInfraredThermometersAchieved { get; set; }
        public int? InstallAdditionalRWISStationsIdentified { get; set; }
        public int? InstallAdditionalRWISStationsAchieved { get; set; }
        public int? AccessRWISDataIdentified { get; set; }
        public int? AccessRWISDataAchieved { get; set; }
        public int? InstallMobileRWISIdentified { get; set; }
        public int? InstallMobileRWISAchieved { get; set; }
        public int? AccessMeteorologicalServiceIdentified { get; set; }
        public int? AccessMeteorologicalServiceAchieved { get; set; }
        public int? AdoptPreWettingMajorityNetworkIdentified { get; set; }
        public int? AdoptPreWettingMajorityNetworkAchieved { get; set; }
        public int? UsePreTreatedSaltIdentified { get; set; }
        public int? UsePreTreatedSaltAchieved { get; set; }
        public int? AdoptPreWettingOrTreatmentAbrasivesIdentified { get; set; }
        public int? AdoptPreWettingOrTreatmentAbrasivesAchieved { get; set; }
        public int? TestingNewProductsIdentified { get; set; }
        public int? TestingNewProductsAchieved { get; set; }
        public int? AdoptAntiIcingStandardIdentified { get; set; }
        public int? AdoptAntiIcingStandardAchieved { get; set; }
        public int? InstallGPSAndComputerSystemsIdentified { get; set; }
        public int? InstallGPSAndComputerSystemsAchieved { get; set; }
        public int? UseChartForApplicationRatesIdentified { get; set; }
        public int? UseChartForApplicationRatesAchieved { get; set; }
        public int? UseMDSSIdentified { get; set; }
        public int? UseMDSSAchieved { get; set; }
        public int? ReviewSaltUseIdentified { get; set; }
        public int? ReviewSaltUseAchieved { get; set; }
        public int? AssessPlowingEfficiencyIdentified { get; set; }
        public int? AssessPlowingEfficiencyAchieved { get; set; }
        public int? OtherIdentified { get; set; }
        public int? OtherAchieved { get; set; }
        public int? DevelopProgramPhaseOutIdentified { get; set; }
        public int? DevelopProgramPhaseOutAchieved { get; set; }
        public int? InstallNewSiteLowPermeabilityIdentified { get; set; }
        public int? InstallNewSiteLowPermeabilityAchieved { get; set; }
        public int? UpgradeExistingSiteLowPermeabilityIdentified { get; set; }
        public int? UpgradeExistingSiteLowPermeabilityAchieved { get; set; }
        public int? CollectMeltWaterSpecificPointIdentified { get; set; }
        public int? CollectMeltWaterSpecificPointAchieved { get; set; }
        public int? ConstructCollectionPondIdentified { get; set; }
        public int? ConstructCollectionPondAchieved { get; set; }
        public int? OtherSnowDisposalIdentified { get; set; }
        public int? OtherSnowDisposalAchieved { get; set; }
        public int? IdentifySaltVulnerableAreasIdentified { get; set; }
        public int? IdentifySaltVulnerableAreasAchieved { get; set; }
        public int? PrioritizeAreasForAdditionalProtectionIdentified { get; set; }
        public int? PrioritizeAreasForAdditionalProtectionAchieved { get; set; }
        public int? ImplementProtectionMitigationMeasuresIdentified { get; set; }
        public int? ImplementProtectionMitigationMeasuresAchieved { get; set; }
        public int? ConductEnvironmentalMonitoringIdentified { get; set; }
        public int? ConductEnvironmentalMonitoringAchieved { get; set; }
        public int? OtherVulnerableAreasIdentified { get; set; }
        public int? OtherVulnerableAreasAchieved { get; set; }
    }
}