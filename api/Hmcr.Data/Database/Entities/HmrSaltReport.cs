using System;

namespace Hmcr.Data.Database.Entities
{
    public class HmrSaltReport
    {
        public int SaltReportId { get; set; }
        public string ServiceArea { get; set; }
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
        public string MaterialStorageIdentified { get; set; }
        public string MaterialStorageAchieved { get; set; }
        public string SaltApplicationIdentified { get; set; }
        public string SaltApplicationAchieved { get; set; }

        // Section 2
        public int RoadTotalLength { get; set; }
        public int SaltTotalDays { get; set; }

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

        // Section 5
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

        // Section 6
        public bool? SnowDisposalSiteUsed { get; set; }
        public bool? SnowMeltersUsed { get; set; }
        public bool? MeltwaterDisposalMethodUsed { get; set; }
        public string SnowDisposalSiteTotal { get; set; }

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
