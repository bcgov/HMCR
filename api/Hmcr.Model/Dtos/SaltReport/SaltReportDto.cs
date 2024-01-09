using System.Collections.Generic;

namespace Hmcr.Model.Dtos.SaltReport
{
    public class SaltReportDto
    {
        public int SaltReportId { get; set; }
        public string ServiceArea { get; set; }
        public string ContactName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public Sect1Dto Sect1 { get; set; }
        public Sect2Dto Sect2 { get; set; }
        public Sect3Dto Sect3 { get; set; }
        public Sect5Dto Sect5 { get; set; }
        public Sect6Dto Sect6 { get; set; }
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
        public DeicerDto Deicer { get; set; }
        public TreatedAbrasivesDto TreatedAbrasives { get; set; }
        public PrewettingDto Prewetting { get; set; }
        public PretreatmentDto Pretreatment { get; set; }
        public AntiicingDto Antiicing { get; set; }
        public MultiChlorideDto MultiChlorideA { get; set; }
        public MultiChlorideDto MultiChlorideB { get; set; }

        public class DeicerDto
        {
            // Section 3
            public int Nacl { get; set; }
            public int Mgcl2 { get; set; }
            public int Cacl2 { get; set; }
            public int Acetate { get; set; }
        }

        public class TreatedAbrasivesDto
        {
            public int SandstoneDust { get; set; }
            public int Nacl { get; set; }
            public int Mgcl2 { get; set; }
            public int Cacl2 { get; set; }
        }

        public class PrewettingDto
        {
            public int Nacl { get; set; }
            public int Mgcl2 { get; set; }
            public int Cacl2 { get; set; }
            public int Acetate { get; set; }
            public int Nonchloride { get; set; }
        }

        public class PretreatmentDto
        {
            public int Nacl { get; set; }
            public int Mgcl2 { get; set; }
            public int Cacl2 { get; set; }
            public int Acetate { get; set; }
            public int Nonchloride { get; set; }
        }

        public class AntiicingDto
        {
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

    // public class Sect5Dto
    // {
    //     public int NumberOfVehicles { get; set; }
    //     public int VehiclesForSaltApplication { get; set; }
    //     public int VehiclesWithConveyors { get; set; }
    //     public int VehiclesWithPreWettingEquipment { get; set; }
    //     public int VehiclesForDLA { get; set; }
    //     public bool? InfraredThermometerRelied { get; set; }
    //     public bool? MeteorologicalServiceRelied { get; set; }
    //     public bool? FixedRWISStationsRelied { get; set; }
    //     public bool? MobileRWISMountedRelied { get; set; }
    //     public int InfraredThermometerTotal { get; set; }
    //     public int MeteorologicalServiceTotal { get; set; }
    //     public int FixedRWISStationsTotal { get; set; }
    //     public int MobileRWISMountedTotal { get; set; }
    //     public bool? AVLRelied { get; set; }
    //     public bool? SaltApplicationRatesRelied { get; set; }
    //     public bool? ApplicationRateChartRelied { get; set; }
    //     public bool? TestingMDSSRelied { get; set; }
    //     public int AVLTotal { get; set; }
    //     public int SaltApplicationRatesTotal { get; set; }
    //     public int ApplicationRateChartTotal { get; set; }
    //     public int TestingSMDSTotal { get; set; }
    // }

    public class Sect5Dto
    {
        public int NumberOfVehicles { get; set; }
        public int VehiclesForSaltApplication { get; set; }
        public int VehiclesWithConveyors { get; set; }
        public int VehiclesWithPreWettingEquipment { get; set; }
        public int VehiclesForDLA { get; set; }

        public WeatherMonitoringSourcesDto WeatherMonitoringSources { get; set; }
        public MaintenanceDecisionSupportDto MaintenanceDecisionSupport { get; set; }

        public class WeatherMonitoringSourcesDto
        {
            public InfraredThermometerDto InfraredThermometer { get; set; }
            public MeteorologicalServiceDto MeteorologicalService { get; set; }
            public FixedRWISStationsDto FixedRWISStations { get; set; }
            public MobileRWISMountedDto MobileRWISMounted { get; set; }

            public class InfraredThermometerDto
            {
                public bool Relied { get; set; }
                public int Number { get; set; }
            }

            public class MeteorologicalServiceDto
            {
                public bool Relied { get; set; }
            }

            public class FixedRWISStationsDto
            {
                public bool Relied { get; set; }
                public int Number { get; set; }
            }

            public class MobileRWISMountedDto
            {
                public bool Relied { get; set; }
                public int Number { get; set; }
            }
        }

        public class MaintenanceDecisionSupportDto
        {
            public AVLDto AVL { get; set; }
            public SaltApplicationRatesDto SaltApplicationRates { get; set; }
            public ApplicationRateChartDto ApplicationRateChart { get; set; }
            public TestingMDSSDto TestingMDSS { get; set; }

            public class AVLDto
            {
                public bool Relied { get; set; }
                public int Number { get; set; }
            }

            public class SaltApplicationRatesDto
            {
                public bool Relied { get; set; }
                public int Number { get; set; }
            }

            public class ApplicationRateChartDto
            {
                public bool Relied { get; set; }
                public int Number { get; set; }
            }

            public class TestingMDSSDto
            {
                public bool Relied { get; set; }
                public int Number { get; set; }
            }
        }
    }


    public class Sect6Dto
    {
        public bool? SnowDisposalSiteUsed { get; set; }
        public bool? SnowMeltersUsed { get; set; }
        public bool? MeltwaterDisposalMethodUsed { get; set; }
        public string SnowDisposalSiteTotal { get; set; }
    }
}
