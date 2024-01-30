namespace Hmcr.Model.Dtos.SaltReport
{
    public class AppendixDto
    {
        public int AppendixId { get; set; }
        public int SaltReportId { get; set; } // Foreign Key
        public MaterialStorageDto MaterialStorage { get; set; }
        public SaltApplicationDto SaltApplication { get; set; }
        public SnowDisposalDto SnowDisposal { get; set; }
        public VulnerableAreasDto VulnerableAreas { get; set; }

        public class MaterialStorageDto
        {
            public ObjectivesDto NewSaltDomeWithPad { get; set; }
            public ObjectivesDto NewSaltDomeIndoorStorage { get; set; }
            public ObjectivesDto UpgradeSaltStorageSites { get; set; }
            public ObjectivesDto ConstructPermanentCoverStructure { get; set; }
            public ObjectivesDto ImpermeablePadForAbrasives { get; set; }
            public ObjectivesDto ExpandInsideBuildingForAbrasives { get; set; }
            public ObjectivesDto UseTarpsForAbrasives { get; set; }
            public ObjectivesDto ReconfigureStorageCapacity { get; set; }
            public ObjectivesDto ReconfigureOperationFacilities { get; set; }
            public ObjectivesDto DesignAreaForTruckLoading { get; set; }
            public ObjectivesDto ControlTruckLoading { get; set; }
            public ObjectivesDto InstallEquipmentWashBay { get; set; }
            public ObjectivesDto DesignSiteForRunoffControl { get; set; }
            public ObjectivesDto ManageSaltContaminatedWaters { get; set; }
            public ObjectivesDto SpillPreventionPlan { get; set; }
            public ObjectivesDto RemoveContaminatedSnow { get; set; }
            public ObjectivesDto OtherSpecify { get; set; }
        }
        public class SaltApplicationDto
        {
            public ObjectivesDto InstallGroundSpeedControls { get; set; }
            public ObjectivesDto IncreasePreWettingEquipment { get; set; }
            public ObjectivesDto InstallLiquidAntiIcing { get; set; }
            public ObjectivesDto InstallInfraredThermometers { get; set; }
            public ObjectivesDto InstallAdditionalRWISStations { get; set; }
            public ObjectivesDto AccessRWISData { get; set; }
            public ObjectivesDto InstallMobileRWIS { get; set; }
            public ObjectivesDto AccessMeteorologicalService { get; set; }
            public ObjectivesDto AdoptPreWettingMajorityNetwork { get; set; }
            public ObjectivesDto UsePreTreatedSalt { get; set; }
            public ObjectivesDto AdoptPreWettingOrTreatmentAbrasives { get; set; }
            public ObjectivesDto TestingNewProducts { get; set; }
            public ObjectivesDto AdoptAntiIcingStandard { get; set; }
            public ObjectivesDto InstallGPSAndComputerSystems { get; set; }
            public ObjectivesDto UseChartForApplicationRates { get; set; }
            public ObjectivesDto UseMDSS { get; set; }
            public ObjectivesDto ReviewSaltUse { get; set; }
            public ObjectivesDto AssessPlowingEfficiency { get; set; }
            public ObjectivesDto Other { get; set; }
        }
        public class SnowDisposalDto
        {
            public ObjectivesDto DevelopProgramPhaseOut { get; set; }
            public ObjectivesDto InstallNewSiteLowPermeability { get; set; }
            public ObjectivesDto UpgradeExistingSiteLowPermeability { get; set; }
            public ObjectivesDto CollectMeltWaterSpecificPoint { get; set; }
            public ObjectivesDto ConstructCollectionPond { get; set; }
            public ObjectivesDto OtherSnowDisposal { get; set; }
        }
        public class VulnerableAreasDto
        {
            public ObjectivesDto IdentifySaltVulnerableAreas { get; set; }
            public ObjectivesDto PrioritizeAreasForAdditionalProtection { get; set; }
            public ObjectivesDto ImplementProtectionMitigationMeasures { get; set; }
            public ObjectivesDto ConductEnvironmentalMonitoring { get; set; }
            public ObjectivesDto OtherVulnerableAreas { get; set; }
        }

        public class ObjectivesDto
        {
            public int Identified { get; set; }
            public int Achieved { get; set; }
        }
    }
}