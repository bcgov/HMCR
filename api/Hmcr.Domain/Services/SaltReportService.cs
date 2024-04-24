using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.SaltReport;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Hmcr.Data.Database.Entities;
using System.IO;
using CsvHelper;
using System.Globalization;
using Hmcr.Domain.CsvHelpers;
using System;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using Hmcr.Data.Database;
using Hmcr.Model.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Hmcr.Domain.Services
{
    public interface ISaltReportService
    {
        Task<HmrSaltReport> CreateReportAsync(SaltReportDto dto);
        Task<IEnumerable<SaltReportDto>> GetAllSaltReportDtosAsync();
        Task<SaltReportDto> GetSaltReportByIdAsync(int saltReportId);
        Task<IEnumerable<HmrSaltReport>> GetSaltReportEntitiesAsync(string serviceAreas, DateTime fromDate, DateTime toDate);
        Task<PagedDto<SaltReportDto>> GetSaltReportDtosAsync(string serviceAreas, DateTime? fromDate, DateTime? toDate, int pageSize, int pageNumber);
        Stream ConvertToCsvStream(IEnumerable<HmrSaltReport> saltReportEntities);
    }

    public class SaltReportService : ISaltReportService
    {
        private readonly ISaltReportRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<SaltReportService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;

        public SaltReportService(AppDbContext context, ISaltReportRepository repository, IMapper mapper, ILogger<SaltReportService> logger, IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context;
        }

        public async Task<HmrSaltReport> CreateReportAsync(SaltReportDto dto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _logger.LogInformation("Mapping main report");
                    var saltReport = MapToEntity(dto);  // Only map the main report here

                    _logger.LogInformation("Adding main report entity to repository");
                    _context.ChangeTracker.AutoDetectChangesEnabled = false;
                    _context.HmrSaltReports.Add(saltReport);
                    _context.ChangeTracker.AutoDetectChangesEnabled = true;

                    // Assuming saltReport.Stockpiles and saltReport.Appendix are populated
                    // Detach the nested entities so they aren't tracked or saved by EF
                    if (saltReport.Stockpiles != null)
                    {
                        foreach (var stockpile in saltReport.Stockpiles)
                        {
                            _context.Entry(stockpile).State = EntityState.Detached;
                        }
                    }

                    if (saltReport.Appendix != null)
                    {
                        _context.Entry(saltReport.Appendix).State = EntityState.Detached;
                    }
                    _unitOfWork.Commit();

                    _logger.LogInformation("Handling dependent entities");

                    if (dto.Sect4.Stockpiles != null && dto.Sect4.Stockpiles.Any())
                    {
                        _logger.LogInformation("Mapping stockpiles");
                        var stockpiles = MapToStockpiles(dto.Sect4.Stockpiles);  // Map stockpiles using AutoMapper
                        foreach (var stockpile in stockpiles)
                        {
                            stockpile.SaltReportId = saltReport.SaltReportId;  // Set the foreign key
                        }
                        _logger.LogInformation("Adding stockpiles to repository");
                        await _context.HmrSaltStockpiles.AddRangeAsync(stockpiles);
                        _unitOfWork.Commit();  // Commit stockpiles
                    }

                    if (dto.Appendix != null)
                    {
                        _logger.LogInformation("Mapping and adding appendix");
                        var appendix = MapToAppendix(dto.Appendix, saltReport.SaltReportId);  // Use the updated mapping method
                        await _context.HmrSaltReportAppendixes.AddAsync(appendix);
                        _unitOfWork.Commit();  // Commit appendix
                    }

                    transaction.Commit();
                    return saltReport;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in creating salt report, rolling back transaction");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }


        private HmrSaltReport MapToEntity(SaltReportDto dto) => _mapper.Map<HmrSaltReport>(dto);
        private HmrSaltReportAppendix MapToAppendix(AppendixDto dto, decimal saltReportId)
        {
            // Use AutoMapper to map from the DTO to the entity
            var appendix = _mapper.Map<HmrSaltReportAppendix>(dto);

            // Manually set the SaltReportId to link the appendix to the report
            appendix.SaltReportId = saltReportId;

            return appendix;
        }

        public List<HmrSaltStockpile> MapToStockpiles(List<StockpileDto> stockpileDtos)
        {
            // Ensure the list is not null before attempting to map
            if (stockpileDtos == null) throw new ArgumentNullException(nameof(stockpileDtos));

            // Use AutoMapper to map the list of DTOs to the list of entities
            var stockpiles = _mapper.Map<List<HmrSaltStockpile>>(stockpileDtos);
            return stockpiles;
        }


        public void ValidateDto(SaltReportDto dto)
        {
            // Validation Logic
        }

        public void ProcessBusinessRules(HmrSaltReport saltReport)
        {
            // Business Logic
        }

        public class RepositoryException : Exception
        {
            public RepositoryException() { }

            public RepositoryException(string message) : base(message) { }

            public RepositoryException(string message, Exception innerException) : base(message, innerException) { }
        }

        public async Task<IEnumerable<SaltReportDto>> GetAllSaltReportDtosAsync()
        {
            try
            {
                var saltReportEntities = await _repository.GetAllReportsAsync();
                _logger.LogInformation($"saltReportEntities.Count, {saltReportEntities.Count()} entities");

                return _mapper.Map<IEnumerable<SaltReportDto>>(saltReportEntities);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument in GetReportsAsync.");
                throw;
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Database exception occurred in GetReportsAsync.");
                throw new RepositoryException("An error occurred while retrieving reports.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetReportsAsync.");
                throw; // Re-throw exception to propagate it up the call stack
            }

        }

        public async Task<PagedDto<SaltReportDto>> GetSaltReportDtosAsync(string serviceAreas, DateTime? fromDate, DateTime? toDate, int pageSize, int pageNumber)
        {
            try
            {
                var saltReports = await _repository.GetPagedReportsAsync(serviceAreas, fromDate, toDate, pageSize, pageNumber).ConfigureAwait(false);
                _logger.LogWarning("Salt report dtos received.");

                return saltReports;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument in GetReportsAsync.");
                throw;
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Database exception occurred in GetReportsAsync.");
                throw new RepositoryException("An error occurred while retrieving reports.", ex);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while getting salt report DTOs.");
                // Return an empty enumerable or handle the error in another way
                throw;
            }
        }


        public async Task<IEnumerable<HmrSaltReport>> GetSaltReportEntitiesAsync(string serviceAreas, DateTime fromDate, DateTime toDate)
        {
            return await _repository.GetReportsAsync(serviceAreas, fromDate, toDate);
        }

        public async Task<SaltReportDto> GetSaltReportByIdAsync(int saltReportId)
        {
            var saltReportEntity = await _repository.GetReportByIdAsync(saltReportId);

            return saltReportEntity != null ? _mapper.Map<SaltReportDto>(saltReportEntity) : null;
        }

        public Stream ConvertToCsvStream(IEnumerable<HmrSaltReport> saltReportEntities)
        {
            var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, leaveOpen: true);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            WriteCsvHeaders(writer);
            RegisterCsvConverters(csv);
            csv.WriteRecords(saltReportEntities);

            WriteTotals(writer, saltReportEntities);

            writer.Flush();
            memoryStream.Position = 0;

            return memoryStream;
        }

        private void WriteCsvHeaders(StreamWriter writer)
        {
            // Headers
            writer.WriteLine("2022-23,,,,1. Salt Management Plan,,,,,,,,,,,,2. Winter Ops,,3. Materials Applied,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,4. Design & Operation at Road Salt Storage sites,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,5. Salt Application,,,,,,,,,,,,,,,,,,,,,,6. Snow Disposal,,,,7. Management of Salt Vulnerable Areas,,,,,,,,,,,,,,,,,,,");
            writer.WriteLine(",,,,Salt Management Plan,,,1.4 Training offered to:,,,,,1.5 Objectives:,,,,(2.1 not used),,3.1 Quantity of materials used:,,,,,,,,,,,,,,,,,,,,,,,3.2 Multi-Chloride Liquids,,,,,,,,4.1,4.2 Stockpile Conditions,,,,,,,,4.3 Good Housekeeping Practices,,,,,,,,,,,,,,,,,,,,,,5.1 Management of Equipment,,,,,,,5.2 Weather Monitoring,,,,,,,5.3 Maintenance Decision Support,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,");
            writer.WriteLine(",,,,1.1,1.2,1.3,Managers,Superv's,Operators,Mechanics,Patrollers,Storage Facilities,,Salt Application,,2.2 Total length,2.3 # of days that,SOLIDS:,,,,,,,,LIQUIDS:,wet sand/salt as it goes out,,,,stop the sand from freezing (goes on the stockpile),,,,,to anti-ice (prevent bond); de-ice (too late),,,,,,,,,,,,,Total #  of,Road Salts,,,,Treated Abrasive,,,,Materials handled on imperm surface,,Truck overload prevention,,Truck wash-water collection system,,Control & diversion of external (non-salt) water,,Drainage inside with collection systems for runoff of salt contaminated waters:,,Discharged into:,,,,,,,,Ongoing cleanup and sweeping,,Risk mgmt & emerg meas plans in place,,# Vehicles used for salt application,,,,,,,Sources of information,,,,,,,Types of systems used to aid decision making,,,,,,,,6.1,,6.2,6.3,Salt Vulnerable Areas,,,,,,,,,,,,,,,,,,,");
            writer.WriteLine(",,,,developed,reviewed,updated,,,,,,#,#,#,#,of roads that,salt was,De-icers,,,,Treated Abrasives,,,,Pre-wetting liquid,,,,,Pre-treatment liquid,,,,,Direct Liquid Application,,,,,Mix A,,,,Mix B,,,,Salt Storage,# salt,Impermeable Surface,Permanent Roof,Tarp Only,total #,Impermeable Surface,Permanent Roof,Tarp Only,,,,,,,,,,,Municipal sewer system,,Containment for removal,,Watercourse,,Other,,,,,,Total #,,conveyor & grnd,,direct,Regular Calibration?,,Infrared Thermometer,,Weather Srv,Fixed RWIS,,Mobile RWIS,,Auto Vehicle Locate,,Record Salt App rates,,Chart for app rates,,Testing of MDSS,,Perform snow disposal at desginated site?,,Use snow melters?,Meltwater discharged in storm sewer?,Inventory?,Vulnerable areas?,Action Plan?,Mitigation Measures,Monitoring?,Drinking Water # Identified,Drinking Water # with protection,Drinking Water # Chloride ,Aquatic Life # Identified,Aquatic Life # with protection,Aquatic Life # Chloride ,Wetlands & Associated aquatic life # Identified,Wetlands & Associated aquatic life # with protection,Wetlands & Associated aquatic life # Chloride ,Delimited Areas w/ terrestrial Fauna/flora # Identified,Delimited Areas w/ terrestrial Fauna/flora # with protection,Delimited Areas w/ terrestrial Fauna/flora # Chloride ,Valued Lands # Identified,Valued Lands # with protection,Valued Lands # Chloride ");
            writer.WriteLine("SA,Contact Name,Number,Email,Y/N?,Y/N?,Y/N?,Y/N?,Y/N?,Y/N?,Y/N?,Y/N?,Identifed,Achieved,Identifed,Achieved,are salted?,applied?,NaCl,MgCl2,CaCl2,Acetate,sand/etc,NaCl,MgCl2,CaCl2,NaCl,MgCl2,CaCl2,Acetate,non-chloride,NaCl,MgCl2,CaCl2,Acetate,non-chloride,NaCl,MgCl2,CaCl2,Acetate,non-chloride,Litres,NaCl %,MgCl2 %,CaCl2 %,Litres,NaCl %,MgCl2 %,CaCl2 %,sites,stockpiles,# sites,# sites,# sites,stockpiles,# sites,# sites,# sites,Y/N,# sites,Y/N,# sites,Y/N,# sites,Y/N,#sites,Y/N,# sites,Y/N,# sites,Y/N,# sites,Y/N,# sites,Y/N,# sites,Y/N,# sites,Y/N,# sites,of vehicles,solid salt,speed sensor,pre-wetting,liquid app,Y/N,#/Yr,Y/N,#,Y/N,Y/N,#,Y/N,#,Y/N,#,Y/N,#,Y/N,#,Y/N,#,Y/N,#,Y/N,Y/N,Y/N,Y/N,Y/N,Y/N,Y/N,#,#,#,#,#,#,#,#,#,#,#,#,#,#,#");
        }

        private void WriteTotals(StreamWriter writer, IEnumerable<HmrSaltReport> saltReportEntities)
        {
            var reports = saltReportEntities;
            var totalReports = reports.Count();

            var TotalPlanDeveloped = reports.GetTotalYesResponses(report => report.PlanDeveloped);
            var TotalPlanReviewed = reports.GetTotalYesResponses(report => report.PlanReviewed);
            var TotalPlanUpdated = reports.GetTotalYesResponses(report => report.PlanUpdated);
            var TotalManagerTraining = reports.GetTotalYesResponses(report => report.ManagerTraining);
            var TotalSupervisorTraining = reports.GetTotalYesResponses(report => report.SupervisorTraining);
            var TotalOperatorTraining = reports.GetTotalYesResponses(report => report.OperatorTraining);
            var TotalMechanicalTraining = reports.GetTotalYesResponses(report => report.MechanicalTraining);
            var TotalPatrollerTraining = reports.GetTotalYesResponses(report => report.PatrollerTraining);
            var TotalMaterialStorageIdentified = reports.GetTotalIntValue(report => report.MaterialStorageIdentified);
            var TotalMaterialStorageAchieved = reports.GetTotalIntValue(report => report.MaterialStorageAchieved);
            var TotalSaltApplicationIdentified = reports.GetTotalIntValue(report => report.SaltApplicationIdentified);
            var TotalSaltApplicationAchieved = reports.GetTotalIntValue(report => report.SaltApplicationAchieved);
            var TotalRoadTotalLength = reports.GetTotalIntValue(report => report.RoadTotalLength);
            var TotalSaltTotalDays = reports.GetTotalIntValue(report => report.SaltTotalDays);
            var TotalDeicerNacl = reports.GetTotalDecimalValue(report => report.DeicerNacl);
            var TotalDeicerMgcl2 = reports.GetTotalDecimalValue(report => report.DeicerMgcl2);
            var TotalDeicerCacl2 = reports.GetTotalDecimalValue(report => report.DeicerCacl2);
            var TotalDeicerAcetate = reports.GetTotalDecimalValue(report => report.DeicerAcetate);
            var TotalTreatedAbrasivesSandstoneDust = reports.GetTotalDecimalValue(report => report.TreatedAbrasivesSandstoneDust);
            var TotalTreatedAbrasivesNacl = reports.GetTotalDecimalValue(report => report.TreatedAbrasivesNacl);
            var TotalTreatedAbrasivesMgcl2 = reports.GetTotalDecimalValue(report => report.TreatedAbrasivesMgcl2);
            var TotalTreatedAbrasivesCacl2 = reports.GetTotalDecimalValue(report => report.TreatedAbrasivesCacl2);
            var TotalPrewettingNacl = reports.GetTotalDecimalValue(report => report.PrewettingNacl);
            var TotalPrewettingMgcl2 = reports.GetTotalDecimalValue(report => report.PrewettingMgcl2);
            var TotalPrewettingCacl2 = reports.GetTotalDecimalValue(report => report.PrewettingCacl2);
            var TotalPrewettingAcetate = reports.GetTotalDecimalValue(report => report.PrewettingAcetate);
            var TotalPrewettingNonchloride = reports.GetTotalDecimalValue(report => report.PrewettingNonchloride);
            var TotalPretreatmentNacl = reports.GetTotalDecimalValue(report => report.PretreatmentNacl);
            var TotalPretreatmentMgcl2 = reports.GetTotalDecimalValue(report => report.PretreatmentMgcl2);
            var TotalPretreatmentCacl2 = reports.GetTotalDecimalValue(report => report.PretreatmentCacl2);
            var TotalPretreatmentAcetate = reports.GetTotalDecimalValue(report => report.PretreatmentAcetate);
            var TotalPretreatmentNonchloride = reports.GetTotalDecimalValue(report => report.PretreatmentNonchloride);
            var TotalAntiicingNacl = reports.GetTotalDecimalValue(report => report.AntiicingNacl);
            var TotalAntiicingMgcl2 = reports.GetTotalDecimalValue(report => report.AntiicingMgcl2);
            var TotalAntiicingCacl2 = reports.GetTotalDecimalValue(report => report.AntiicingCacl2);
            var TotalAntiicingAcetate = reports.GetTotalDecimalValue(report => report.AntiicingAcetate);
            var TotalAntiicingNonchloride = reports.GetTotalDecimalValue(report => report.AntiicingNonchloride);
            var TotalMultichlorideALitres = reports.GetTotalDecimalValue(report => report.MultichlorideALitres);
            var TotalMultichlorideANaclPercentage = reports.GetTotalDecimalValue(report => report.MultichlorideANaclPercentage);
            var TotalMultichlorideAMgcl2Percentage = reports.GetTotalDecimalValue(report => report.MultichlorideAMgcl2Percentage);
            var TotalMultichlorideACacl2Percentage = reports.GetTotalDecimalValue(report => report.MultichlorideACacl2Percentage);
            var TotalMultichlorideBLitres = reports.GetTotalDecimalValue(report => report.MultichlorideBLitres);
            var TotalMultichlorideBNaclPercentage = reports.GetTotalDecimalValue(report => report.MultichlorideBNaclPercentage);
            var TotalMultichlorideBMgcl2Percentage = reports.GetTotalDecimalValue(report => report.MultichlorideBMgcl2Percentage);
            var TotalMultichlorideBCacl2Percentage = reports.GetTotalDecimalValue(report => report.MultichlorideBCacl2Percentage);
            // Section 4
            var TotalSaltStorageSitesTotal = reports.GetTotalIntValue(report => report.SaltStorageSitesTotal);
            var TotalRoadSaltStockpilesTotal = reports.GetTotalIntValue(report => report.RoadSaltStockpilesTotal);
            var TotalRoadSaltOnImpermeableSurface = reports.GetTotalIntValue(report => report.RoadSaltOnImpermeableSurface);
            var TotalRoadSaltUnderPermanentRoof = reports.GetTotalIntValue(report => report.RoadSaltUnderPermanentRoof);
            var TotalRoadSaltUnderTarp = reports.GetTotalIntValue(report => report.RoadSaltUnderTarp);
            var TotalTreatedAbrasivesStockpilesTotal = reports.GetTotalIntValue(report => report.TreatedAbrasivesStockpilesTotal);
            var TotalTreatedAbrasivesOnImpermeableSurface = reports.GetTotalIntValue(report => report.TreatedAbrasivesOnImpermeableSurface);
            var TotalTreatedAbrasivesUnderPermanentRoof = reports.GetTotalIntValue(report => report.TreatedAbrasivesUnderPermanentRoof);
            var TotalTreatedAbrasivesUnderTarp = reports.GetTotalIntValue(report => report.TreatedAbrasivesUnderTarp);

            var TotalAllMaterialsHandledPlan = reports.GetTotalBoolValue(report => report.AllMaterialsHandledPlan);
            var TotalAllMaterialsHandledSites = reports.GetTotalIntValue(report => report.AllMaterialsHandledSites);
            var TotalEquipmentPreventsOverloadingPlan = reports.GetTotalBoolValue(report => report.EquipmentPreventsOverloadingPlan);
            var TotalEquipmentPreventsOverloadingSites = reports.GetTotalIntValue(report => report.EquipmentPreventsOverloadingSites);
            var TotalWastewaterSystemPlan = reports.GetTotalBoolValue(report => report.WastewaterSystemPlan);
            var TotalWastewaterSystemSites = reports.GetTotalIntValue(report => report.WastewaterSystemSites);
            var TotalControlDiversionExternalWatersPlan = reports.GetTotalBoolValue(report => report.ControlDiversionExternalWatersPlan);
            var TotalControlDiversionExternalWatersSites = reports.GetTotalIntValue(report => report.ControlDiversionExternalWatersSites);
            var TotalDrainageCollectionSystemPlan = reports.GetTotalBoolValue(report => report.DrainageCollectionSystemPlan);
            var TotalDrainageCollectionSystemSites = reports.GetTotalIntValue(report => report.DrainageCollectionSystemSites);
            var TotalMunicipalSewerSystemPlan = reports.GetTotalBoolValue(report => report.MunicipalSewerSystemPlan);
            var TotalMunicipalSewerSystemSites = reports.GetTotalIntValue(report => report.MunicipalSewerSystemSites);
            var TotalRemovalContainmentPlan = reports.GetTotalBoolValue(report => report.RemovalContainmentPlan);
            var TotalRemovalContainmentSites = reports.GetTotalIntValue(report => report.RemovalContainmentSites);
            var TotalWatercoursePlan = reports.GetTotalBoolValue(report => report.WatercoursePlan);
            var TotalWatercourseSites = reports.GetTotalIntValue(report => report.WatercourseSites);
            var TotalOtherDischargePointPlan = reports.GetTotalBoolValue(report => report.OtherDischargePointPlan);
            var TotalOtherDischargePointSites = reports.GetTotalIntValue(report => report.OtherDischargePointSites);
            var TotalOngoingCleanupPlan = reports.GetTotalBoolValue(report => report.OngoingCleanupPlan);
            var TotalOngoingCleanupSites = reports.GetTotalIntValue(report => report.OngoingCleanupSites);
            var TotalRiskManagementPlanPlan = reports.GetTotalBoolValue(report => report.RiskManagementPlanPlan);
            var TotalRiskManagementPlanSites = reports.GetTotalIntValue(report => report.RiskManagementPlanSites);
            var TotalNumberOfVehicles = reports.GetTotalIntValue(report => report.NumberOfVehicles);
            var TotalVehiclesForSaltApplication = reports.GetTotalIntValue(report => report.VehiclesForSaltApplication);
            var TotalVehiclesWithConveyors = reports.GetTotalIntValue(report => report.VehiclesWithConveyors);
            var TotalVehiclesWithPreWettingEquipment = reports.GetTotalIntValue(report => report.VehiclesWithPreWettingEquipment);
            var TotalVehiclesForDLA = reports.GetTotalIntValue(report => report.VehiclesForDLA);
            var TotalRegularCalibration = reports.GetTotalBoolValue(report => report.RegularCalibration);
            var TotalRegularCalibrationTotal = reports.GetTotalIntValue(report => report.RegularCalibrationTotal);
            var TotalInfraredThermometerRelied = reports.GetTotalBoolValue(report => report.InfraredThermometerRelied);
            var TotalInfraredThermometerTotal = reports.GetTotalIntValue(report => report.InfraredThermometerTotal);
            var TotalMeteorologicalServiceRelied = reports.GetTotalBoolValue(report => report.MeteorologicalServiceRelied);
            var TotalFixedRWISStationsRelied = reports.GetTotalBoolValue(report => report.FixedRWISStationsRelied);
            var TotalFixedRWISStationsTotal = reports.GetTotalIntValue(report => report.FixedRWISStationsTotal);
            var TotalMobileRWISMountedRelied = reports.GetTotalBoolValue(report => report.MobileRWISMountedRelied);
            var TotalMobileRWISMountedTotal = reports.GetTotalIntValue(report => report.MobileRWISMountedTotal);
            var TotalAVLRelied = reports.GetTotalBoolValue(report => report.AVLRelied);
            var TotalAVLTotal = reports.GetTotalIntValue(report => report.AVLTotal);
            var TotalSaltApplicationRatesRelied = reports.GetTotalBoolValue(report => report.SaltApplicationRatesRelied);
            var TotalSaltApplicationRatesTotal = reports.GetTotalIntValue(report => report.SaltApplicationRatesTotal);
            var TotalApplicationRateChartRelied = reports.GetTotalBoolValue(report => report.ApplicationRateChartRelied);
            var TotalApplicationRateChartTotal = reports.GetTotalIntValue(report => report.ApplicationRateChartTotal);
            var TotalTestingMDSSRelied = reports.GetTotalBoolValue(report => report.TestingMDSSRelied);
            var TotalTestingSMDSTotal = reports.GetTotalIntValue(report => report.TestingSMDSTotal);
            var TotalSnowDisposalSiteUsed = reports.GetTotalBoolValue(report => report.SnowDisposalSiteUsed);
            var TotalSnowDisposalSiteTotal = reports.GetTotalIntValue(report => report.SnowDisposalSiteTotal);
            var TotalSnowMeltersUsed = reports.GetTotalBoolValue(report => report.SnowMeltersUsed);
            var TotalMeltwaterDisposalMethodUsed = reports.GetTotalBoolValue(report => report.MeltwaterDisposalMethodUsed);
            var TotalCompletedInventory = reports.GetTotalYesResponses(report => report.CompletedInventory);
            var TotalSetVulnerableAreas = reports.GetTotalYesResponses(report => report.SetVulnerableAreas);
            var TotalActionPlanPrepared = reports.GetTotalYesResponses(report => report.ActionPlanPrepared);
            var TotalProtectionMeasuresImplemented = reports.GetTotalYesResponses(report => report.ProtectionMeasuresImplemented);
            var TotalEnvironmentalMonitoringConducted = reports.GetTotalYesResponses(report => report.EnvironmentalMonitoringConducted);
            var TotalDrinkingWaterAreasIdentified = reports.GetTotalIntValue(report => report.DrinkingWaterAreasIdentified);
            var TotalDrinkingWaterAreasWithProtection = reports.GetTotalIntValue(report => report.DrinkingWaterAreasWithProtection);
            var TotalDrinkingWaterAreasWithChloride = reports.GetTotalIntValue(report => report.DrinkingWaterAreasWithChloride);
            var TotalAquaticLifeAreasIdentified = reports.GetTotalIntValue(report => report.AquaticLifeAreasIdentified);
            var TotalAquaticLifeAreasWithProtection = reports.GetTotalIntValue(report => report.AquaticLifeAreasWithProtection);
            var TotalAquaticLifeAreasWithChloride = reports.GetTotalIntValue(report => report.AquaticLifeAreasWithChloride);
            var TotalWetlandsAreasIdentified = reports.GetTotalIntValue(report => report.WetlandsAreasIdentified);
            var TotalWetlandsAreasWithProtection = reports.GetTotalIntValue(report => report.WetlandsAreasWithProtection);
            var TotalWetlandsAreasWithChloride = reports.GetTotalIntValue(report => report.WetlandsAreasWithChloride);
            var TotalDelimitedAreasAreasIdentified = reports.GetTotalIntValue(report => report.DelimitedAreasAreasIdentified);
            var TotalDelimitedAreasAreasWithProtection = reports.GetTotalIntValue(report => report.DelimitedAreasAreasWithProtection);
            var TotalDelimitedAreasAreasWithChloride = reports.GetTotalIntValue(report => report.DelimitedAreasAreasWithChloride);
            var TotalValuedLandsAreasIdentified = reports.GetTotalIntValue(report => report.ValuedLandsAreasIdentified);
            var TotalValuedLandsAreasWithProtection = reports.GetTotalIntValue(report => report.ValuedLandsAreasWithProtection);
            var TotalValuedLandsAreasWithChloride = reports.GetTotalIntValue(report => report.ValuedLandsAreasWithChloride);

            writer.WriteLine($"Total / Total Yes,,,,{TotalPlanDeveloped}, {TotalPlanReviewed}, {TotalPlanUpdated}, {TotalManagerTraining}, {TotalSupervisorTraining}, {TotalOperatorTraining}, {TotalMechanicalTraining}, {TotalPatrollerTraining}, {TotalMaterialStorageIdentified}, {TotalMaterialStorageAchieved}, {TotalSaltApplicationIdentified}, {TotalSaltApplicationAchieved}, {TotalRoadTotalLength}, {TotalSaltTotalDays}, {TotalDeicerNacl}, {TotalDeicerMgcl2}, {TotalDeicerCacl2}, {TotalDeicerAcetate}, {TotalTreatedAbrasivesSandstoneDust}, {TotalTreatedAbrasivesNacl}, {TotalTreatedAbrasivesMgcl2}, {TotalTreatedAbrasivesCacl2}, {TotalPrewettingNacl}, {TotalPrewettingMgcl2}, {TotalPrewettingCacl2}, {TotalPrewettingAcetate}, {TotalPrewettingNonchloride}, {TotalPretreatmentNacl}, {TotalPretreatmentMgcl2}, {TotalPretreatmentCacl2}, {TotalPretreatmentAcetate}, {TotalPretreatmentNonchloride}, {TotalAntiicingNacl}, {TotalAntiicingMgcl2}, {TotalAntiicingCacl2}, {TotalAntiicingAcetate}, {TotalAntiicingNonchloride}, {TotalMultichlorideALitres}, {TotalMultichlorideANaclPercentage}, {TotalMultichlorideAMgcl2Percentage}, {TotalMultichlorideACacl2Percentage}, {TotalMultichlorideBLitres}, {TotalMultichlorideBNaclPercentage}, {TotalMultichlorideBMgcl2Percentage}, {TotalMultichlorideBCacl2Percentage},{TotalSaltStorageSitesTotal},{TotalRoadSaltStockpilesTotal},{TotalRoadSaltOnImpermeableSurface},{TotalRoadSaltUnderPermanentRoof},{TotalRoadSaltUnderTarp},{TotalTreatedAbrasivesStockpilesTotal},{TotalTreatedAbrasivesOnImpermeableSurface},{TotalTreatedAbrasivesUnderPermanentRoof},{TotalTreatedAbrasivesUnderTarp}, {TotalAllMaterialsHandledPlan}, {TotalAllMaterialsHandledSites}, {TotalEquipmentPreventsOverloadingPlan}, {TotalEquipmentPreventsOverloadingSites}, {TotalWastewaterSystemPlan}, {TotalWastewaterSystemSites}, {TotalControlDiversionExternalWatersPlan}, {TotalControlDiversionExternalWatersSites}, {TotalDrainageCollectionSystemPlan}, {TotalDrainageCollectionSystemSites}, {TotalMunicipalSewerSystemPlan}, {TotalMunicipalSewerSystemSites}, {TotalRemovalContainmentPlan}, {TotalRemovalContainmentSites}, {TotalWatercoursePlan}, {TotalWatercourseSites}, {TotalOtherDischargePointPlan}, {TotalOtherDischargePointSites}, {TotalOngoingCleanupPlan}, {TotalOngoingCleanupSites}, {TotalRiskManagementPlanPlan}, {TotalRiskManagementPlanSites}, {TotalNumberOfVehicles}, {TotalVehiclesForSaltApplication}, {TotalVehiclesWithConveyors}, {TotalVehiclesWithPreWettingEquipment}, {TotalVehiclesForDLA}, {TotalRegularCalibration}, {TotalRegularCalibrationTotal}, {TotalInfraredThermometerRelied}, {TotalInfraredThermometerTotal}, {TotalMeteorologicalServiceRelied}, {TotalFixedRWISStationsRelied}, {TotalFixedRWISStationsTotal}, {TotalMobileRWISMountedRelied}, {TotalMobileRWISMountedTotal}, {TotalAVLRelied}, {TotalAVLTotal}, {TotalSaltApplicationRatesRelied}, {TotalSaltApplicationRatesTotal}, {TotalApplicationRateChartRelied}, {TotalApplicationRateChartTotal}, {TotalTestingMDSSRelied}, {TotalTestingSMDSTotal}, {TotalSnowDisposalSiteUsed}, {TotalSnowDisposalSiteTotal}, {TotalSnowMeltersUsed}, {TotalMeltwaterDisposalMethodUsed}, {TotalCompletedInventory}, {TotalSetVulnerableAreas}, {TotalActionPlanPrepared}, {TotalProtectionMeasuresImplemented}, {TotalEnvironmentalMonitoringConducted}, {TotalDrinkingWaterAreasIdentified}, {TotalDrinkingWaterAreasWithProtection}, {TotalDrinkingWaterAreasWithChloride}, {TotalAquaticLifeAreasIdentified}, {TotalAquaticLifeAreasWithProtection}, {TotalAquaticLifeAreasWithChloride}, {TotalWetlandsAreasIdentified}, {TotalWetlandsAreasWithProtection}, {TotalWetlandsAreasWithChloride}, {TotalDelimitedAreasAreasIdentified}, {TotalDelimitedAreasAreasWithProtection}, {TotalDelimitedAreasAreasWithChloride}, {TotalValuedLandsAreasIdentified}, {TotalValuedLandsAreasWithProtection}, {TotalValuedLandsAreasWithChloride}");
            writer.WriteLine($"Total No (treats empties as No),,,,{totalReports - TotalPlanDeveloped}, {totalReports - TotalPlanReviewed}, {totalReports - TotalPlanUpdated}, {totalReports - TotalManagerTraining}, {totalReports - TotalSupervisorTraining}, {totalReports - TotalOperatorTraining}, {totalReports - TotalMechanicalTraining}, {totalReports - TotalPatrollerTraining}, , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , , {totalReports - TotalAllMaterialsHandledPlan}, , {totalReports - TotalEquipmentPreventsOverloadingPlan}, , {totalReports - TotalWastewaterSystemPlan}, , {totalReports - TotalControlDiversionExternalWatersPlan}, , {totalReports - TotalDrainageCollectionSystemPlan}, , {totalReports - TotalMunicipalSewerSystemPlan}, , {totalReports - TotalRemovalContainmentPlan}, , {totalReports - TotalWatercoursePlan}, , {totalReports - TotalOtherDischargePointPlan}, , {totalReports - TotalOngoingCleanupPlan}, , {totalReports - TotalRiskManagementPlanPlan},,,,,,,{totalReports - TotalRegularCalibration}, , {totalReports - TotalInfraredThermometerRelied}, , {totalReports - TotalMeteorologicalServiceRelied}, {totalReports - TotalFixedRWISStationsRelied}, ,{totalReports - TotalMobileRWISMountedRelied}, , {totalReports - TotalAVLRelied}, , {totalReports - TotalSaltApplicationRatesRelied}, , {totalReports - TotalApplicationRateChartRelied}, , {totalReports - TotalTestingMDSSRelied}, , {totalReports - TotalSnowDisposalSiteUsed}, , {totalReports - TotalSnowMeltersUsed}, , {totalReports - TotalCompletedInventory}, {totalReports - TotalSetVulnerableAreas}, {totalReports - TotalActionPlanPrepared}, {totalReports - TotalProtectionMeasuresImplemented}, {totalReports - TotalEnvironmentalMonitoringConducted}");
        }

        private void RegisterCsvConverters(CsvWriter csv)
        {
            // Add custom boolean converter
            csv.Configuration.TypeConverterCache.AddConverter<bool>(new BooleanYesNoConverter());
            // For nullable booleans
            csv.Configuration.TypeConverterCache.AddConverter<bool?>(new BooleanYesNoConverter());

            // Register the mapping
            csv.Configuration.RegisterClassMap<SaltReportMap>();
            csv.Configuration.HasHeaderRecord = false;
        }

    }
}