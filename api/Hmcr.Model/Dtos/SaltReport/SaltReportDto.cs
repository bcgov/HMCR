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
    }

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
    }

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

    public class Sect2Dto
    {
        public int RoadTotalLength { get; set; }
        public int SaltTotalDays { get; set; }
    }

    public class Sect3Dto
    {
        // Section 3
        public int DeicerNacl { get; set; }
        public int DeicerMgcl2 { get; set; }
        public int DeicerCacl2 { get; set; }
        public int DeicerAcetate { get; set; }

        public int TreatedAbrasivesSandstoneDust { get; set; }
        public int TreatedAbrasivesNacl { get; set; }
        public int TreatedAbrasivesMgcl2 { get; set; }
        public int TreatedAbrasivesCacl2 { get; set; }

        public int PrewettingNacl { get; set; }
        public int PrewettingMgcl2 { get; set; }
        public int PrewettingCacl2 { get; set; }
        public int PrewettingAcetate { get; set; }
        public int PrewettingNonchloride { get; set; }

        public int PretreatmentNacl { get; set; }
        public int PretreatmentMgcl2 { get; set; }
        public int PretreatmentCacl2 { get; set; }
        public int PretreatmentAcetate { get; set; }
        public int PretreatmentNonchloride { get; set; }

        public int AntiicingNacl { get; set; }
        public int AntiicingMgcl2 { get; set; }
        public int AntiicingCacl2 { get; set; }
        public int AntiicingAcetate { get; set; }
        public int AntiicingNonchloride { get; set; }

        public int MultichlorideALitres { get; set; }
        public int MultichlorideANaclPercentage { get; set; }
        public int MultichlorideAMgcl2Percentage { get; set; }
        public int MultichlorideACacl2Percentage { get; set; }
        public int MultichlorideBLitres { get; set; }
        public int MultichlorideBNaclPercentage { get; set; }
        public int MultichlorideBMgcl2Percentage { get; set; }
        public int MultichlorideBCacl2Percentage { get; set; }
    }

    public class Sect5Dto
    {
        public int NumberOfVehicles { get; set; }
        public int VehiclesForSaltApplication { get; set; }
        public int VehiclesWithConveyors { get; set; }
        public int VehiclesWithPreWettingEquipment { get; set; }
        public int VehiclesForDLA { get; set; }
        public bool? InfraredThermometerRelied { get; set; }
        public bool? MeteorologicalServiceRelied { get; set; }
        public bool? FixedRWISStationsRelied { get; set; }
        public bool? MobileRWISMountedRelied { get; set; }
        public int InfraredThermometerTotal { get; set; }
        public int MeteorologicalServiceTotal { get; set; }
        public int FixedRWISStationsTotal { get; set; }
        public int MobileRWISMountedTotal { get; set; }
        public bool? AVLRelied { get; set; }
        public bool? SaltApplicationRatesRelied { get; set; }
        public bool? ApplicationRateChartRelied { get; set; }
        public bool? TestingMDSSRelied { get; set; }
        public int AVLTotal { get; set; }
        public int SaltApplicationRatesTotal { get; set; }
        public int ApplicationRateChartTotal { get; set; }
        public int TestingSMDSTotal { get; set; }
    }

    public class Sect6Dto
    {
        public bool? SnowDisposalSiteUsed { get; set; }
        public bool? SnowMeltersUsed { get; set; }
        public bool? MeltwaterDisposalMethodUsed { get; set; }
        public string SnowDisposalSiteTotal { get; set; }
    }
}
