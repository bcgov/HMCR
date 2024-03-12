using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Model.Dtos.LocationCode;
using Hmcr.Model.Dtos.CodeLookup;
using Hmcr.Model.Dtos.ContractTerm;
using Hmcr.Model.Dtos.District;
using Hmcr.Model.Dtos.FeedbackMessage;
using Hmcr.Model.Dtos.MimeType;
using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.Permission;
using Hmcr.Model.Dtos.Region;
using Hmcr.Model.Dtos.RockfallReport;
using Hmcr.Model.Dtos.Role;
using Hmcr.Model.Dtos.RolePermission;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.ServiceAreaUser;
using Hmcr.Model.Dtos.ServiceAreaActivity;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Dtos.SubmissionStatus;
using Hmcr.Model.Dtos.SubmissionStream;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Dtos.UserRole;
using Hmcr.Model.Dtos.WildlifeReport;
using Hmcr.Model.Dtos.WorkReport;
using Hmcr.Model.Dtos.ActivityCode;
using Hmcr.Model.Dtos.ActivityRule;
using Hmcr.Model.Dtos.SaltReport;

namespace Hmcr.Data.Mappings
{
    public class ModelToEntityProfile : Profile
    {
        public ModelToEntityProfile()
        {
            //SourceMemberNamingConvention = new PascalCaseNamingConvention();
            //DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();

            SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            DestinationMemberNamingConvention = new PascalCaseNamingConvention();

            CreateMap<ContractTermDto, HmrContractTerm>();

            CreateMap<DistrictDto, HmrDistrict>();

            CreateMap<MimeTypeDto, HmrMimeType>();

            CreateMap<PartyDto, HmrParty>();

            CreateMap<PermissionDto, HmrPermission>();

            CreateMap<RegionDto, HmrRegion>();

            CreateMap<RoleDto, HmrRole>();
            CreateMap<RoleCreateDto, HmrRole>();
            CreateMap<RoleUpdateDto, HmrRole>();
            CreateMap<RoleSearchDto, HmrRole>();
            CreateMap<RoleDeleteDto, HmrRole>();

            CreateMap<RolePermissionDto, HmrRolePermission>();

            CreateMap<ServiceAreaDto, HmrServiceArea>();
            CreateMap<ServiceAreaNumberDto, HmrServiceArea>();

            CreateMap<ServiceAreaUserDto, HmrServiceAreaUser>();

            CreateMap<SubmissionObjectDto, HmrSubmissionObject>();
            CreateMap<SubmissionObjectCreateDto, HmrSubmissionObject>();

            CreateMap<SubmissionRowDto, HmrSubmissionRow>();

            CreateMap<SubmissionStatusDto, HmrSubmissionStatu>();

            CreateMap<UserDto, HmrSystemUser>();
            CreateMap<UserCreateDto, HmrSystemUser>();
            CreateMap<UserCurrentDto, HmrSystemUser>();
            CreateMap<UserSearchDto, HmrSystemUser>();
            CreateMap<UserUpdateDto, HmrSystemUser>();
            CreateMap<UserDeleteDto, HmrSystemUser>();
            CreateMap<UserSearchExportDto, HmrSystemUser>();
            CreateMap<UserRoleDto, HmrUserRole>();

            CreateMap<SubmissionStreamDto, HmrSubmissionStream>();

            CreateMap<ActivityCodeDto, HmrActivityCode>();
            CreateMap<ActivityCodeSearchDto, HmrActivityCode>();
            CreateMap<ActivityCodeCreateDto, HmrActivityCode>();
            CreateMap<ActivityCodeUpdateDto, HmrActivityCode>();
            CreateMap<ActivityCodeSearchExportDto, HmrActivityCode>();

            CreateMap<LocationCodeDto, HmrLocationCode>();

            CreateMap<WorkReportTyped, HmrWorkReport>();

            CreateMap<RockfallReportTyped, HmrRockfallReport>()
                .ForMember(dst => dst.ReporterName, opt => opt.MapFrom(src => src.Name));

            CreateMap<WildlifeReportTyped, HmrWildlifeReport>();

            CreateMap<CodeLookupDto, HmrCodeLookup>();

            CreateMap<FeedbackMessageDto, HmrFeedbackMessage>();
            CreateMap<FeedbackMessageUpdateDto, HmrFeedbackMessage>();

            CreateMap<ActivityCodeRuleDto, HmrActivityCodeRule>();

            CreateMap<ServiceAreaActivityDto, HmrServiceAreaActivity>();

            CreateMap<SaltReportDto, HmrSaltReport>()
                // Section 1
                .ForMember(dest => dest.ServiceArea, opt => opt.MapFrom(src => src.ServiceArea))
                .ForMember(dest => dest.ContactName, opt => opt.MapFrom(src => src.ContactName))
                .ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src.Telephone))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PlanDeveloped, opt => opt.MapFrom(src => src.Sect1.PlanDeveloped))
                .ForMember(dest => dest.PlanReviewed, opt => opt.MapFrom(src => src.Sect1.PlanReviewed))
                .ForMember(dest => dest.PlanUpdated, opt => opt.MapFrom(src => src.Sect1.PlanUpdated))
                .ForMember(dest => dest.ManagerTraining, opt => opt.MapFrom(src => src.Sect1.Training.Manager))
                .ForMember(dest => dest.SupervisorTraining, opt => opt.MapFrom(src => src.Sect1.Training.Supervisor))
                .ForMember(dest => dest.OperatorTraining, opt => opt.MapFrom(src => src.Sect1.Training.Operator))
                .ForMember(dest => dest.MechanicalTraining, opt => opt.MapFrom(src => src.Sect1.Training.Mechanical))
                .ForMember(dest => dest.PatrollerTraining, opt => opt.MapFrom(src => src.Sect1.Training.Patroller))
                .ForMember(dest => dest.MaterialStorageAchieved, opt => opt.MapFrom(src => src.Sect1.Objectives.MaterialStorage.Achieved))
                .ForMember(dest => dest.MaterialStorageIdentified, opt => opt.MapFrom(src => src.Sect1.Objectives.MaterialStorage.Identified))
                .ForMember(dest => dest.SaltApplicationAchieved, opt => opt.MapFrom(src => src.Sect1.Objectives.SaltApplication.Achieved))
                .ForMember(dest => dest.SaltApplicationIdentified, opt => opt.MapFrom(src => src.Sect1.Objectives.SaltApplication.Identified))
                // Section 2
                .ForMember(dest => dest.RoadTotalLength, opt => opt.MapFrom(src => src.Sect2.RoadTotalLength))
                .ForMember(dest => dest.SaltTotalDays, opt => opt.MapFrom(src => src.Sect2.SaltTotalDays))
                // Section 3
                .ForMember(dest => dest.DeicerNacl, opt => opt.MapFrom(src => src.Sect3.Deicer.Nacl))
                .ForMember(dest => dest.DeicerMgcl2, opt => opt.MapFrom(src => src.Sect3.Deicer.Mgcl2))
                .ForMember(dest => dest.DeicerCacl2, opt => opt.MapFrom(src => src.Sect3.Deicer.Cacl2))
                .ForMember(dest => dest.DeicerAcetate, opt => opt.MapFrom(src => src.Sect3.Deicer.Acetate))
                .ForMember(dest => dest.TreatedAbrasivesSandstoneDust, opt => opt.MapFrom(src => src.Sect3.TreatedAbrasives.SandstoneDust))
                .ForMember(dest => dest.TreatedAbrasivesNacl, opt => opt.MapFrom(src => src.Sect3.TreatedAbrasives.Nacl))
                .ForMember(dest => dest.TreatedAbrasivesMgcl2, opt => opt.MapFrom(src => src.Sect3.TreatedAbrasives.Mgcl2))
                .ForMember(dest => dest.TreatedAbrasivesCacl2, opt => opt.MapFrom(src => src.Sect3.TreatedAbrasives.Cacl2))
                .ForMember(dest => dest.PrewettingNacl, opt => opt.MapFrom(src => src.Sect3.Prewetting.Nacl))
                .ForMember(dest => dest.PrewettingMgcl2, opt => opt.MapFrom(src => src.Sect3.Prewetting.Mgcl2))
                .ForMember(dest => dest.PrewettingCacl2, opt => opt.MapFrom(src => src.Sect3.Prewetting.Cacl2))
                .ForMember(dest => dest.PrewettingAcetate, opt => opt.MapFrom(src => src.Sect3.Prewetting.Acetate))
                .ForMember(dest => dest.PrewettingNonchloride, opt => opt.MapFrom(src => src.Sect3.Prewetting.Nonchloride))
                .ForMember(dest => dest.PretreatmentNacl, opt => opt.MapFrom(src => src.Sect3.Pretreatment.Nacl))
                .ForMember(dest => dest.PretreatmentMgcl2, opt => opt.MapFrom(src => src.Sect3.Pretreatment.Mgcl2))
                .ForMember(dest => dest.PretreatmentCacl2, opt => opt.MapFrom(src => src.Sect3.Pretreatment.Cacl2))
                .ForMember(dest => dest.PretreatmentAcetate, opt => opt.MapFrom(src => src.Sect3.Pretreatment.Acetate))
                .ForMember(dest => dest.PretreatmentNonchloride, opt => opt.MapFrom(src => src.Sect3.Pretreatment.Nonchloride))
                .ForMember(dest => dest.AntiicingNacl, opt => opt.MapFrom(src => src.Sect3.Antiicing.Nacl))
                .ForMember(dest => dest.AntiicingMgcl2, opt => opt.MapFrom(src => src.Sect3.Antiicing.Mgcl2))
                .ForMember(dest => dest.AntiicingCacl2, opt => opt.MapFrom(src => src.Sect3.Antiicing.Cacl2))
                .ForMember(dest => dest.AntiicingAcetate, opt => opt.MapFrom(src => src.Sect3.Antiicing.Acetate))
                .ForMember(dest => dest.AntiicingNonchloride, opt => opt.MapFrom(src => src.Sect3.Antiicing.Nonchloride))
                .ForMember(dest => dest.MultichlorideALitres, opt => opt.MapFrom(src => src.Sect3.MultiChlorideA.Litres))
                .ForMember(dest => dest.MultichlorideANaclPercentage, opt => opt.MapFrom(src => src.Sect3.MultiChlorideA.NaclPercentage))
                .ForMember(dest => dest.MultichlorideAMgcl2Percentage, opt => opt.MapFrom(src => src.Sect3.MultiChlorideA.Mgcl2Percentage))
                .ForMember(dest => dest.MultichlorideACacl2Percentage, opt => opt.MapFrom(src => src.Sect3.MultiChlorideA.Cacl2Percentage))
                .ForMember(dest => dest.MultichlorideBLitres, opt => opt.MapFrom(src => src.Sect3.MultiChlorideB.Litres))
                .ForMember(dest => dest.MultichlorideBNaclPercentage, opt => opt.MapFrom(src => src.Sect3.MultiChlorideB.NaclPercentage))
                .ForMember(dest => dest.MultichlorideBMgcl2Percentage, opt => opt.MapFrom(src => src.Sect3.MultiChlorideB.Mgcl2Percentage))
                .ForMember(dest => dest.MultichlorideBCacl2Percentage, opt => opt.MapFrom(src => src.Sect3.MultiChlorideB.Cacl2Percentage))
                // Section 4
                .ForMember(dest => dest.SaltStorageSitesTotal, opt => opt.MapFrom(src => src.Sect4.SaltStorageSitesTotal))
                .ForMember(dest => dest.AllMaterialsHandledPlan, opt => opt.MapFrom(src => src.Sect4.Practices.AllMaterialsHandled.HasPlan))
                .ForMember(dest => dest.AllMaterialsHandledSites, opt => opt.MapFrom(src => src.Sect4.Practices.AllMaterialsHandled.NumSites))
                .ForMember(dest => dest.EquipmentPreventsOverloadingPlan, opt => opt.MapFrom(src => src.Sect4.Practices.EquipmentPreventsOverloading.HasPlan))
                .ForMember(dest => dest.EquipmentPreventsOverloadingSites, opt => opt.MapFrom(src => src.Sect4.Practices.EquipmentPreventsOverloading.NumSites))
                .ForMember(dest => dest.WastewaterSystemPlan, opt => opt.MapFrom(src => src.Sect4.Practices.WastewaterSystem.HasPlan))
                .ForMember(dest => dest.WastewaterSystemSites, opt => opt.MapFrom(src => src.Sect4.Practices.WastewaterSystem.NumSites))
                .ForMember(dest => dest.ControlDiversionExternalWatersPlan, opt => opt.MapFrom(src => src.Sect4.Practices.ControlDiversionExternalWaters.HasPlan))
                .ForMember(dest => dest.ControlDiversionExternalWatersSites, opt => opt.MapFrom(src => src.Sect4.Practices.ControlDiversionExternalWaters.NumSites))
                .ForMember(dest => dest.DrainageCollectionSystemPlan, opt => opt.MapFrom(src => src.Sect4.Practices.DrainageCollectionSystem.HasPlan))
                .ForMember(dest => dest.DrainageCollectionSystemSites, opt => opt.MapFrom(src => src.Sect4.Practices.DrainageCollectionSystem.NumSites))
                .ForMember(dest => dest.OngoingCleanupPlan, opt => opt.MapFrom(src => src.Sect4.Practices.OngoingCleanup.HasPlan))
                .ForMember(dest => dest.OngoingCleanupSites, opt => opt.MapFrom(src => src.Sect4.Practices.OngoingCleanup.NumSites))
                .ForMember(dest => dest.RiskManagementPlanPlan, opt => opt.MapFrom(src => src.Sect4.Practices.RiskManagementPlan.HasPlan))
                .ForMember(dest => dest.RiskManagementPlanSites, opt => opt.MapFrom(src => src.Sect4.Practices.RiskManagementPlan.NumSites))
                .ForMember(dest => dest.MunicipalSewerSystemPlan, opt => opt.MapFrom(src => src.Sect4.Practices.MunicipalSewerSystem.HasPlan))
                .ForMember(dest => dest.MunicipalSewerSystemSites, opt => opt.MapFrom(src => src.Sect4.Practices.MunicipalSewerSystem.NumSites))
                .ForMember(dest => dest.RemovalContainmentPlan, opt => opt.MapFrom(src => src.Sect4.Practices.RemovalContainment.HasPlan))
                .ForMember(dest => dest.RemovalContainmentSites, opt => opt.MapFrom(src => src.Sect4.Practices.RemovalContainment.NumSites))
                .ForMember(dest => dest.WatercoursePlan, opt => opt.MapFrom(src => src.Sect4.Practices.Watercourse.HasPlan))
                .ForMember(dest => dest.WatercourseSites, opt => opt.MapFrom(src => src.Sect4.Practices.Watercourse.NumSites))
                .ForMember(dest => dest.OtherDischargePointPlan, opt => opt.MapFrom(src => src.Sect4.Practices.OtherDischargePoint.HasPlan))
                .ForMember(dest => dest.OtherDischargePointSites, opt => opt.MapFrom(src => src.Sect4.Practices.OtherDischargePoint.NumSites))
                .ForMember(dest => dest.Stockpiles, opt => opt.MapFrom(src => src.Sect4.Stockpiles))
                // Section 5
                .ForMember(dest => dest.NumberOfVehicles, opt => opt.MapFrom(src => src.Sect5.NumberOfVehicles))
                .ForMember(dest => dest.VehiclesForSaltApplication, opt => opt.MapFrom(src => src.Sect5.VehiclesForSaltApplication))
                .ForMember(dest => dest.VehiclesWithConveyors, opt => opt.MapFrom(src => src.Sect5.VehiclesWithConveyors))
                .ForMember(dest => dest.VehiclesWithPreWettingEquipment, opt => opt.MapFrom(src => src.Sect5.VehiclesWithPreWettingEquipment))
                .ForMember(dest => dest.VehiclesForDLA, opt => opt.MapFrom(src => src.Sect5.VehiclesForDLA))
                .ForMember(dest => dest.RegularCalibration, opt => opt.MapFrom(src => src.Sect5.RegularCalibration))
                .ForMember(dest => dest.RegularCalibrationTotal, opt => opt.MapFrom(src => src.Sect5.RegularCalibrationTotal))
                .ForMember(dest => dest.InfraredThermometerRelied, opt => opt.MapFrom(src => src.Sect5.WeatherMonitoringSources.InfraredThermometer.Relied))
                .ForMember(dest => dest.MeteorologicalServiceRelied, opt => opt.MapFrom(src => src.Sect5.WeatherMonitoringSources.MeteorologicalService.Relied))
                .ForMember(dest => dest.FixedRWISStationsRelied, opt => opt.MapFrom(src => src.Sect5.WeatherMonitoringSources.FixedRWISStations.Relied))
                .ForMember(dest => dest.MobileRWISMountedRelied, opt => opt.MapFrom(src => src.Sect5.WeatherMonitoringSources.MobileRWISMounted.Relied))
                .ForMember(dest => dest.InfraredThermometerTotal, opt => opt.MapFrom(src => src.Sect5.WeatherMonitoringSources.InfraredThermometer.Number))
                .ForMember(dest => dest.FixedRWISStationsTotal, opt => opt.MapFrom(src => src.Sect5.WeatherMonitoringSources.FixedRWISStations.Number))
                .ForMember(dest => dest.MobileRWISMountedTotal, opt => opt.MapFrom(src => src.Sect5.WeatherMonitoringSources.MobileRWISMounted.Number))
                .ForMember(dest => dest.AVLRelied, opt => opt.MapFrom(src => src.Sect5.MaintenanceDecisionSupport.AVL.Relied))
                .ForMember(dest => dest.SaltApplicationRatesRelied, opt => opt.MapFrom(src => src.Sect5.MaintenanceDecisionSupport.SaltApplicationRates.Relied))
                .ForMember(dest => dest.ApplicationRateChartRelied, opt => opt.MapFrom(src => src.Sect5.MaintenanceDecisionSupport.ApplicationRateChart.Relied))
                .ForMember(dest => dest.TestingMDSSRelied, opt => opt.MapFrom(src => src.Sect5.MaintenanceDecisionSupport.TestingMDSS.Relied))
                .ForMember(dest => dest.AVLTotal, opt => opt.MapFrom(src => src.Sect5.MaintenanceDecisionSupport.AVL.Number))
                .ForMember(dest => dest.SaltApplicationRatesTotal, opt => opt.MapFrom(src => src.Sect5.MaintenanceDecisionSupport.SaltApplicationRates.Number))
                .ForMember(dest => dest.ApplicationRateChartTotal, opt => opt.MapFrom(src => src.Sect5.MaintenanceDecisionSupport.ApplicationRateChart.Number))
                .ForMember(dest => dest.TestingSMDSTotal, opt => opt.MapFrom(src => src.Sect5.MaintenanceDecisionSupport.TestingMDSS.Number))
                // Section 6
                .ForMember(dest => dest.SnowDisposalSiteUsed, opt => opt.MapFrom(src => src.Sect6.Disposal.Used))
                .ForMember(dest => dest.SnowMeltersUsed, opt => opt.MapFrom(src => src.Sect6.SnowMelter.Used))
                .ForMember(dest => dest.MeltwaterDisposalMethodUsed, opt => opt.MapFrom(src => src.Sect6.Meltwater.Used))
                .ForMember(dest => dest.SnowDisposalSiteTotal, opt => opt.MapFrom(src => src.Sect6.Disposal.Total))
                // Section 7
                .ForMember(dest => dest.CompletedInventory, opt => opt.MapFrom(src => src.Sect7.CompletedInventory))
                .ForMember(dest => dest.SetVulnerableAreas, opt => opt.MapFrom(src => src.Sect7.SetVulnerableAreas))
                .ForMember(dest => dest.ActionPlanPrepared, opt => opt.MapFrom(src => src.Sect7.ActionPlanPrepared))
                .ForMember(dest => dest.ProtectionMeasuresImplemented, opt => opt.MapFrom(src => src.Sect7.ProtectionMeasuresImplemented))
                .ForMember(dest => dest.EnvironmentalMonitoringConducted, opt => opt.MapFrom(src => src.Sect7.EnvironmentalMonitoringConducted))
                .ForMember(dest => dest.DrinkingWaterAreasIdentified, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.DrinkingWater.AreasIdentified))
                .ForMember(dest => dest.DrinkingWaterAreasWithProtection, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.DrinkingWater.AreasWithProtection))
                .ForMember(dest => dest.DrinkingWaterAreasWithChloride, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.DrinkingWater.AreasWithChloride))
                .ForMember(dest => dest.AquaticLifeAreasIdentified, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.AquaticLife.AreasIdentified))
                .ForMember(dest => dest.AquaticLifeAreasWithProtection, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.AquaticLife.AreasWithProtection))
                .ForMember(dest => dest.AquaticLifeAreasWithChloride, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.AquaticLife.AreasWithChloride))
                .ForMember(dest => dest.WetlandsAreasIdentified, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.Wetlands.AreasIdentified))
                .ForMember(dest => dest.WetlandsAreasWithProtection, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.Wetlands.AreasWithProtection))
                .ForMember(dest => dest.WetlandsAreasWithChloride, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.Wetlands.AreasWithChloride))
                .ForMember(dest => dest.DelimitedAreasAreasIdentified, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.DelimitedAreas.AreasIdentified))
                .ForMember(dest => dest.DelimitedAreasAreasWithProtection, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.DelimitedAreas.AreasWithProtection))
                .ForMember(dest => dest.DelimitedAreasAreasWithChloride, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.DelimitedAreas.AreasWithChloride))
                .ForMember(dest => dest.ValuedLandsAreasIdentified, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.ValuedLands.AreasIdentified))
                .ForMember(dest => dest.ValuedLandsAreasWithProtection, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.ValuedLands.AreasWithProtection))
                .ForMember(dest => dest.ValuedLandsAreasWithChloride, opt => opt.MapFrom(src => src.Sect7.TypesOfVulnerableAreas.ValuedLands.AreasWithChloride))
                .ForMember(dest => dest.Appendix, opt => opt.MapFrom(src => src.Appendix))
                .ForMember(dest => dest.AppCreateTimestamp, opt => opt.MapFrom(src => src.AppCreateTimestamp))
                .ReverseMap();

            CreateMap<StockpileDto, HmrSaltStockpile>()
                .ForMember(dest => dest.SiteName, opt => opt.MapFrom(src => src.SiteName))
                .ForMember(dest => dest.MotiOwned, opt => opt.MapFrom(src => src.MotiOwned))
                .ForMember(dest => dest.RoadSaltStockpilesTotal, opt => opt.MapFrom(src => src.RoadSalts.StockpilesTotal))
                .ForMember(dest => dest.RoadSaltOnImpermeableSurface, opt => opt.MapFrom(src => src.RoadSalts.OnImpermeableSurface))
                .ForMember(dest => dest.RoadSaltUnderPermanentRoof, opt => opt.MapFrom(src => src.RoadSalts.UnderPermanentRoof))
                .ForMember(dest => dest.RoadSaltUnderTarp, opt => opt.MapFrom(src => src.RoadSalts.UnderTarp))
                .ForMember(dest => dest.TreatedAbrasivesStockpilesTotal, opt => opt.MapFrom(src => src.TreatedAbrasives.StockpilesTotal))
                .ForMember(dest => dest.TreatedAbrasivesOnImpermeableSurface, opt => opt.MapFrom(src => src.TreatedAbrasives.OnImpermeableSurface))
                .ForMember(dest => dest.TreatedAbrasivesUnderPermanentRoof, opt => opt.MapFrom(src => src.TreatedAbrasives.UnderPermanentRoof))
                .ForMember(dest => dest.TreatedAbrasivesUnderTarp, opt => opt.MapFrom(src => src.TreatedAbrasives.UnderTarp))
                .ReverseMap();

            CreateMap<AppendixDto, HmrSaltReportAppendix>()
                .ForMember(dest => dest.NewSaltDomeWithPadIdentified, opt => opt.MapFrom(src => src.MaterialStorage.NewSaltDomeWithPad.Identified))
                .ForMember(dest => dest.NewSaltDomeWithPadAchieved, opt => opt.MapFrom(src => src.MaterialStorage.NewSaltDomeWithPad.Achieved))
                .ForMember(dest => dest.NewSaltDomeIndoorStorageIdentified, opt => opt.MapFrom(src => src.MaterialStorage.NewSaltDomeIndoorStorage.Identified))
                .ForMember(dest => dest.NewSaltDomeIndoorStorageAchieved, opt => opt.MapFrom(src => src.MaterialStorage.NewSaltDomeIndoorStorage.Achieved))
                .ForMember(dest => dest.UpgradeSaltStorageSitesIdentified, opt => opt.MapFrom(src => src.MaterialStorage.UpgradeSaltStorageSites.Identified))
                .ForMember(dest => dest.UpgradeSaltStorageSitesAchieved, opt => opt.MapFrom(src => src.MaterialStorage.UpgradeSaltStorageSites.Achieved))
                .ForMember(dest => dest.ConstructPermanentCoverStructureIdentified, opt => opt.MapFrom(src => src.MaterialStorage.ConstructPermanentCoverStructure.Identified))
                .ForMember(dest => dest.ConstructPermanentCoverStructureAchieved, opt => opt.MapFrom(src => src.MaterialStorage.ConstructPermanentCoverStructure.Achieved))
                .ForMember(dest => dest.ImpermeablePadForAbrasivesIdentified, opt => opt.MapFrom(src => src.MaterialStorage.ImpermeablePadForAbrasives.Identified))
                .ForMember(dest => dest.ImpermeablePadForAbrasivesAchieved, opt => opt.MapFrom(src => src.MaterialStorage.ImpermeablePadForAbrasives.Achieved))
                .ForMember(dest => dest.ExpandInsideBuildingForAbrasivesIdentified, opt => opt.MapFrom(src => src.MaterialStorage.ExpandInsideBuildingForAbrasives.Identified))
                .ForMember(dest => dest.ExpandInsideBuildingForAbrasivesAchieved, opt => opt.MapFrom(src => src.MaterialStorage.ExpandInsideBuildingForAbrasives.Achieved))
                .ForMember(dest => dest.UseTarpsForAbrasivesIdentified, opt => opt.MapFrom(src => src.MaterialStorage.UseTarpsForAbrasives.Identified))
                .ForMember(dest => dest.UseTarpsForAbrasivesAchieved, opt => opt.MapFrom(src => src.MaterialStorage.UseTarpsForAbrasives.Achieved))
                .ForMember(dest => dest.ReconfigureStorageCapacityIdentified, opt => opt.MapFrom(src => src.MaterialStorage.ReconfigureStorageCapacity.Identified))
                .ForMember(dest => dest.ReconfigureStorageCapacityAchieved, opt => opt.MapFrom(src => src.MaterialStorage.ReconfigureStorageCapacity.Achieved))
                .ForMember(dest => dest.ReconfigureOperationFacilitiesIdentified, opt => opt.MapFrom(src => src.MaterialStorage.ReconfigureOperationFacilities.Identified))
                .ForMember(dest => dest.ReconfigureOperationFacilitiesAchieved, opt => opt.MapFrom(src => src.MaterialStorage.ReconfigureOperationFacilities.Achieved))
                .ForMember(dest => dest.DesignAreaForTruckLoadingIdentified, opt => opt.MapFrom(src => src.MaterialStorage.DesignAreaForTruckLoading.Identified))
                .ForMember(dest => dest.DesignAreaForTruckLoadingAchieved, opt => opt.MapFrom(src => src.MaterialStorage.DesignAreaForTruckLoading.Achieved))
                .ForMember(dest => dest.ControlTruckLoadingIdentified, opt => opt.MapFrom(src => src.MaterialStorage.ControlTruckLoading.Identified))
                .ForMember(dest => dest.ControlTruckLoadingAchieved, opt => opt.MapFrom(src => src.MaterialStorage.ControlTruckLoading.Achieved))
                .ForMember(dest => dest.InstallEquipmentWashBayIdentified, opt => opt.MapFrom(src => src.MaterialStorage.InstallEquipmentWashBay.Identified))
                .ForMember(dest => dest.InstallEquipmentWashBayAchieved, opt => opt.MapFrom(src => src.MaterialStorage.InstallEquipmentWashBay.Achieved))
                .ForMember(dest => dest.DesignSiteForRunoffControlIdentified, opt => opt.MapFrom(src => src.MaterialStorage.DesignSiteForRunoffControl.Identified))
                .ForMember(dest => dest.DesignSiteForRunoffControlAchieved, opt => opt.MapFrom(src => src.MaterialStorage.DesignSiteForRunoffControl.Achieved))
                .ForMember(dest => dest.ManageSaltContaminatedWatersIdentified, opt => opt.MapFrom(src => src.MaterialStorage.ManageSaltContaminatedWaters.Identified))
                .ForMember(dest => dest.ManageSaltContaminatedWatersAchieved, opt => opt.MapFrom(src => src.MaterialStorage.ManageSaltContaminatedWaters.Achieved))
                .ForMember(dest => dest.SpillPreventionPlanIdentified, opt => opt.MapFrom(src => src.MaterialStorage.SpillPreventionPlan.Identified))
                .ForMember(dest => dest.SpillPreventionPlanAchieved, opt => opt.MapFrom(src => src.MaterialStorage.SpillPreventionPlan.Achieved))
                .ForMember(dest => dest.RemoveContaminatedSnowIdentified, opt => opt.MapFrom(src => src.MaterialStorage.RemoveContaminatedSnow.Identified))
                .ForMember(dest => dest.RemoveContaminatedSnowAchieved, opt => opt.MapFrom(src => src.MaterialStorage.RemoveContaminatedSnow.Achieved))
                .ForMember(dest => dest.OtherSpecifyIdentified, opt => opt.MapFrom(src => src.MaterialStorage.OtherSpecify.Identified))
                .ForMember(dest => dest.OtherSpecifyAchieved, opt => opt.MapFrom(src => src.MaterialStorage.OtherSpecify.Achieved))
                .ForMember(dest => dest.InstallGroundSpeedControlsIdentified, opt => opt.MapFrom(src => src.SaltApplication.InstallGroundSpeedControls.Identified))
                .ForMember(dest => dest.InstallGroundSpeedControlsAchieved, opt => opt.MapFrom(src => src.SaltApplication.InstallGroundSpeedControls.Achieved))
                .ForMember(dest => dest.IncreasePreWettingEquipmentIdentified, opt => opt.MapFrom(src => src.SaltApplication.IncreasePreWettingEquipment.Identified))
                .ForMember(dest => dest.IncreasePreWettingEquipmentAchieved, opt => opt.MapFrom(src => src.SaltApplication.IncreasePreWettingEquipment.Achieved))
                .ForMember(dest => dest.InstallLiquidAntiIcingIdentified, opt => opt.MapFrom(src => src.SaltApplication.InstallLiquidAntiIcing.Identified))
                .ForMember(dest => dest.InstallLiquidAntiIcingAchieved, opt => opt.MapFrom(src => src.SaltApplication.InstallLiquidAntiIcing.Achieved))
                .ForMember(dest => dest.InstallInfraredThermometersIdentified, opt => opt.MapFrom(src => src.SaltApplication.InstallInfraredThermometers.Identified))
                .ForMember(dest => dest.InstallInfraredThermometersAchieved, opt => opt.MapFrom(src => src.SaltApplication.InstallInfraredThermometers.Achieved))
                .ForMember(dest => dest.InstallAdditionalRWISStationsIdentified, opt => opt.MapFrom(src => src.SaltApplication.InstallAdditionalRWISStations.Identified))
                .ForMember(dest => dest.InstallAdditionalRWISStationsAchieved, opt => opt.MapFrom(src => src.SaltApplication.InstallAdditionalRWISStations.Achieved))
                .ForMember(dest => dest.AccessRWISDataIdentified, opt => opt.MapFrom(src => src.SaltApplication.AccessRWISData.Identified))
                .ForMember(dest => dest.AccessRWISDataAchieved, opt => opt.MapFrom(src => src.SaltApplication.AccessRWISData.Achieved))
                .ForMember(dest => dest.InstallMobileRWISIdentified, opt => opt.MapFrom(src => src.SaltApplication.InstallMobileRWIS.Identified))
                .ForMember(dest => dest.InstallMobileRWISAchieved, opt => opt.MapFrom(src => src.SaltApplication.InstallMobileRWIS.Achieved))
                .ForMember(dest => dest.AccessMeteorologicalServiceIdentified, opt => opt.MapFrom(src => src.SaltApplication.AccessMeteorologicalService.Identified))
                .ForMember(dest => dest.AccessMeteorologicalServiceAchieved, opt => opt.MapFrom(src => src.SaltApplication.AccessMeteorologicalService.Achieved))
                .ForMember(dest => dest.AdoptPreWettingMajorityNetworkIdentified, opt => opt.MapFrom(src => src.SaltApplication.AdoptPreWettingMajorityNetwork.Identified))
                .ForMember(dest => dest.AdoptPreWettingMajorityNetworkAchieved, opt => opt.MapFrom(src => src.SaltApplication.AdoptPreWettingMajorityNetwork.Achieved))
                .ForMember(dest => dest.UsePreTreatedSaltIdentified, opt => opt.MapFrom(src => src.SaltApplication.UsePreTreatedSalt.Identified))
                .ForMember(dest => dest.UsePreTreatedSaltAchieved, opt => opt.MapFrom(src => src.SaltApplication.UsePreTreatedSalt.Achieved))
                .ForMember(dest => dest.AdoptPreWettingOrTreatmentAbrasivesIdentified, opt => opt.MapFrom(src => src.SaltApplication.AdoptPreWettingOrTreatmentAbrasives.Identified))
                .ForMember(dest => dest.AdoptPreWettingOrTreatmentAbrasivesAchieved, opt => opt.MapFrom(src => src.SaltApplication.AdoptPreWettingOrTreatmentAbrasives.Achieved))
                .ForMember(dest => dest.TestingNewProductsIdentified, opt => opt.MapFrom(src => src.SaltApplication.TestingNewProducts.Identified))
                .ForMember(dest => dest.TestingNewProductsAchieved, opt => opt.MapFrom(src => src.SaltApplication.TestingNewProducts.Achieved))
                .ForMember(dest => dest.AdoptAntiIcingStandardIdentified, opt => opt.MapFrom(src => src.SaltApplication.AdoptAntiIcingStandard.Identified))
                .ForMember(dest => dest.AdoptAntiIcingStandardAchieved, opt => opt.MapFrom(src => src.SaltApplication.AdoptAntiIcingStandard.Achieved))
                .ForMember(dest => dest.InstallGPSAndComputerSystemsIdentified, opt => opt.MapFrom(src => src.SaltApplication.InstallGPSAndComputerSystems.Identified))
                .ForMember(dest => dest.InstallGPSAndComputerSystemsAchieved, opt => opt.MapFrom(src => src.SaltApplication.InstallGPSAndComputerSystems.Achieved))
                .ForMember(dest => dest.UseChartForApplicationRatesIdentified, opt => opt.MapFrom(src => src.SaltApplication.UseChartForApplicationRates.Identified))
                .ForMember(dest => dest.UseChartForApplicationRatesAchieved, opt => opt.MapFrom(src => src.SaltApplication.UseChartForApplicationRates.Achieved))
                .ForMember(dest => dest.UseMDSSIdentified, opt => opt.MapFrom(src => src.SaltApplication.UseMDSS.Identified))
                .ForMember(dest => dest.UseMDSSAchieved, opt => opt.MapFrom(src => src.SaltApplication.UseMDSS.Achieved))
                .ForMember(dest => dest.ReviewSaltUseIdentified, opt => opt.MapFrom(src => src.SaltApplication.ReviewSaltUse.Identified))
                .ForMember(dest => dest.ReviewSaltUseAchieved, opt => opt.MapFrom(src => src.SaltApplication.ReviewSaltUse.Achieved))
                .ForMember(dest => dest.AssessPlowingEfficiencyIdentified, opt => opt.MapFrom(src => src.SaltApplication.AssessPlowingEfficiency.Identified))
                .ForMember(dest => dest.AssessPlowingEfficiencyAchieved, opt => opt.MapFrom(src => src.SaltApplication.AssessPlowingEfficiency.Achieved))
                .ForMember(dest => dest.OtherIdentified, opt => opt.MapFrom(src => src.SaltApplication.Other.Identified))
                .ForMember(dest => dest.OtherAchieved, opt => opt.MapFrom(src => src.SaltApplication.Other.Achieved))
                .ForMember(dest => dest.DevelopProgramPhaseOutIdentified, opt => opt.MapFrom(src => src.SnowDisposal.DevelopProgramPhaseOut.Identified))
                .ForMember(dest => dest.DevelopProgramPhaseOutAchieved, opt => opt.MapFrom(src => src.SnowDisposal.DevelopProgramPhaseOut.Achieved))
                .ForMember(dest => dest.InstallNewSiteLowPermeabilityIdentified, opt => opt.MapFrom(src => src.SnowDisposal.InstallNewSiteLowPermeability.Identified))
                .ForMember(dest => dest.InstallNewSiteLowPermeabilityAchieved, opt => opt.MapFrom(src => src.SnowDisposal.InstallNewSiteLowPermeability.Achieved))
                .ForMember(dest => dest.UpgradeExistingSiteLowPermeabilityIdentified, opt => opt.MapFrom(src => src.SnowDisposal.UpgradeExistingSiteLowPermeability.Identified))
                .ForMember(dest => dest.UpgradeExistingSiteLowPermeabilityAchieved, opt => opt.MapFrom(src => src.SnowDisposal.UpgradeExistingSiteLowPermeability.Achieved))
                .ForMember(dest => dest.CollectMeltWaterSpecificPointIdentified, opt => opt.MapFrom(src => src.SnowDisposal.CollectMeltWaterSpecificPoint.Identified))
                .ForMember(dest => dest.CollectMeltWaterSpecificPointAchieved, opt => opt.MapFrom(src => src.SnowDisposal.CollectMeltWaterSpecificPoint.Achieved))
                .ForMember(dest => dest.ConstructCollectionPondIdentified, opt => opt.MapFrom(src => src.SnowDisposal.ConstructCollectionPond.Identified))
                .ForMember(dest => dest.ConstructCollectionPondAchieved, opt => opt.MapFrom(src => src.SnowDisposal.ConstructCollectionPond.Achieved))
                .ForMember(dest => dest.OtherSnowDisposalIdentified, opt => opt.MapFrom(src => src.SnowDisposal.OtherSnowDisposal.Identified))
                .ForMember(dest => dest.OtherSnowDisposalAchieved, opt => opt.MapFrom(src => src.SnowDisposal.OtherSnowDisposal.Achieved))
                .ForMember(dest => dest.IdentifySaltVulnerableAreasIdentified, opt => opt.MapFrom(src => src.VulnerableAreas.IdentifySaltVulnerableAreas.Identified))
                .ForMember(dest => dest.IdentifySaltVulnerableAreasAchieved, opt => opt.MapFrom(src => src.VulnerableAreas.IdentifySaltVulnerableAreas.Achieved))
                .ForMember(dest => dest.PrioritizeAreasForAdditionalProtectionIdentified, opt => opt.MapFrom(src => src.VulnerableAreas.PrioritizeAreasForAdditionalProtection.Identified))
                .ForMember(dest => dest.PrioritizeAreasForAdditionalProtectionAchieved, opt => opt.MapFrom(src => src.VulnerableAreas.PrioritizeAreasForAdditionalProtection.Achieved))
                .ForMember(dest => dest.ImplementProtectionMitigationMeasuresIdentified, opt => opt.MapFrom(src => src.VulnerableAreas.ImplementProtectionMitigationMeasures.Identified))
                .ForMember(dest => dest.ImplementProtectionMitigationMeasuresAchieved, opt => opt.MapFrom(src => src.VulnerableAreas.ImplementProtectionMitigationMeasures.Achieved))
                .ForMember(dest => dest.ConductEnvironmentalMonitoringIdentified, opt => opt.MapFrom(src => src.VulnerableAreas.ConductEnvironmentalMonitoring.Identified))
                .ForMember(dest => dest.ConductEnvironmentalMonitoringAchieved, opt => opt.MapFrom(src => src.VulnerableAreas.ConductEnvironmentalMonitoring.Achieved))
                .ForMember(dest => dest.OtherVulnerableAreasIdentified, opt => opt.MapFrom(src => src.VulnerableAreas.OtherVulnerableAreas.Identified))
                .ForMember(dest => dest.OtherVulnerableAreasAchieved, opt => opt.MapFrom(src => src.VulnerableAreas.OtherVulnerableAreas.Achieved))
                .ReverseMap();
        }
    }
}
