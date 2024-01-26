using System.Collections.Generic;
using Hmcr.Model.Dtos.ServiceArea;

namespace Hmcr.Model.Dtos.SaltReport
{
    public class SaltReportDto
    {
        public int SaltReportId { get; set; }
        public int ServiceArea { get; set; }
        public string ContactName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public Sect1Dto Sect1 { get; set; }
        public Sect2Dto Sect2 { get; set; }
        public Sect3Dto Sect3 { get; set; }
        public Sect4Dto Sect4 { get; set; }
        public Sect5Dto Sect5 { get; set; }
        public Sect6Dto Sect6 { get; set; }
        public Sect7Dto Sect7 { get; set; }
    }

    public class Sect1Dto
    {
        public string PlanDeveloped { get; set; }
        public string PlanReviewed { get; set; }
        public string PlanUpdated { get; set; }
        public TrainingDto Training { get; set; }
        public ObjectivesDto Objectives { get; set; }

        public class TrainingDto
        {
            public string Manager { get; set; }
            public string Supervisor { get; set; }
            public string Operator { get; set; }
            public string Mechanical { get; set; }
            public string Patroller { get; set; }
        }

        public class ObjectivesDto
        {
            public MaterialStorageDto MaterialStorage { get; set; }
            public SaltApplicationDto SaltApplication { get; set; }

            public class MaterialStorageDto
            {
                public string Identified { get; set; }
                public string Achieved { get; set; }
            }

            public class SaltApplicationDto
            {
                public string Identified { get; set; }
                public string Achieved { get; set; }
            }
        }
    }

    public class Sect2Dto
    {
        public int RoadTotalLength { get; set; }
        public int SaltTotalDays { get; set; }
    }

    public class Sect3Dto
    {
        public MaterialDto Deicer { get; set; }
        public MaterialDto TreatedAbrasives { get; set; }
        public MaterialDto Prewetting { get; set; }
        public MaterialDto Pretreatment { get; set; }
        public MaterialDto Antiicing { get; set; }
        public MultiChlorideDto MultiChlorideA { get; set; }
        public MultiChlorideDto MultiChlorideB { get; set; }

        public class MaterialDto
        {
            // Section 3
            public int SandstoneDust { get; set; }
            public int Nacl { get; set; }
            public int Mgcl2 { get; set; }
            public int Cacl2 { get; set; }
            public int Acetate { get; set; }
            public int Nonchloride { get; set; }
        }

        public class MultiChlorideDto
        {
            public int Litres { get; set; }
            public int NaclPercentage { get; set; }
            public int Mgcl2Percentage { get; set; }
            public int Cacl2Percentage { get; set; }
        }
    }

    public class Sect4Dto
    {
        public int SaltStorageSitesTotal { get; set; }
        public List<StockpileDto> Stockpiles { get; set; }
        public PracticesDto Practices { get; set; }
        public class PracticesDto
        {
            public class PracticeItemDto
            {
                public string Label { get; set; }
                public bool HasPlan { get; set; }
                public int NumSites { get; set; }
            }
            public PracticeItemDto AllMaterialsHandled { get; set; }
            public PracticeItemDto EquipmentPreventsOverloading { get; set; }
            public PracticeItemDto WastewaterSystem { get; set; }
            public PracticeItemDto ControlDiversionExternalWaters { get; set; }
            public PracticeItemDto DrainageCollectionSystem { get; set; }
            public PracticeItemDto OngoingCleanup { get; set; }
            public PracticeItemDto RiskManagementPlan { get; set; }
            public PracticeItemDto MunicipalSewerSystem { get; set; }
            public PracticeItemDto RemovalContainment { get; set; }
            public PracticeItemDto Watercourse { get; set; }
            public PracticeItemDto OtherDischargePoint { get; set; }
        }
    }

    public class Sect5Dto
    {
        public int NumberOfVehicles { get; set; }
        public int VehiclesForSaltApplication { get; set; }
        public int VehiclesWithConveyors { get; set; }
        public int VehiclesWithPreWettingEquipment { get; set; }
        public int VehiclesForDLA { get; set; }
        public bool? RegularCalibration { get; set; }
        public int? RegularCalibrationTotal { get; set; }

        public WeatherMonitoringSourcesDto WeatherMonitoringSources { get; set; }
        public MaintenanceDecisionSupportDto MaintenanceDecisionSupport { get; set; }

        public class WeatherMonitoringSourcesDto
        {
            public WMSDto InfraredThermometer { get; set; }
            public WMSDto MeteorologicalService { get; set; }
            public WMSDto FixedRWISStations { get; set; }
            public WMSDto MobileRWISMounted { get; set; }

            public class WMSDto
            {
                public bool Relied { get; set; }
                public int Number { get; set; }
            }
        }

        public class MaintenanceDecisionSupportDto
        {
            public MDSDto AVL { get; set; }
            public MDSDto SaltApplicationRates { get; set; }
            public MDSDto ApplicationRateChart { get; set; }
            public MDSDto TestingMDSS { get; set; }

            public class MDSDto
            {
                public bool Relied { get; set; }
                public int Number { get; set; }
            }
        }
    }


    public class Sect6Dto
    {
        public SnowDisposalDto Disposal { get; set; }
        public SnowDisposalDto SnowMelter { get; set; }
        public SnowDisposalDto Meltwater { get; set; }

        public class SnowDisposalDto
        {
            public bool? Used { get; set; }
            public int Total { get; set; }
        }
    }

    public class Sect7Dto
    {
        public string CompletedInventory { get; set; }
        public string SetVulnerableAreas { get; set; }
        public string ActionPlanPrepared { get; set; }
        public string ProtectionMeasuresImplemented { get; set; }
        public string EnvironmentalMonitoringConducted { get; set; }
        public TypesOfVulnerableAreasDto TypesOfVulnerableAreas { get; set; }

        public class TypesOfVulnerableAreasDto
        {
            public AreaDto DrinkingWater { get; set; }
            public AreaDto AquaticLife { get; set; }
            public AreaDto Wetlands { get; set; }
            public AreaDto DelimitedAreas { get; set; }
            public AreaDto ValuedLands { get; set; }

            public class AreaDto
            {
                public int? AreasIdentified { get; set; }
                public int? AreasWithProtection { get; set; }
                public int? AreasWithChloride { get; set; }
            }
        }
    }
}
