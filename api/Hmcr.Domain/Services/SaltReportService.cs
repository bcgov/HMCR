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
using iText.Kernel.Pdf;
using iText.Forms;
using iText.Forms.Fields;
using System.Reflection;
using iText.Layout;
using Org.BouncyCastle.Crypto.Agreement;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Hmcr.Domain.Services
{
    public interface ISaltReportService
    {
        Task<HmrSaltReport> CreateReportAsync(SaltReportDto dto);
        Task<IEnumerable<SaltReportDto>> GetAllSaltReportDtosAsync();
        Task<SaltReportDto> GetSaltReportByIdAsync(int saltReportId);
        Task<IEnumerable<HmrSaltReport>> GetSaltReportEntitiesAsync(string serviceAreas, DateTime fromDate, DateTime toDate);
        Task<PagedDto<SaltReportDto>> GetSaltReportDtosAsync(string serviceAreas, DateTime? fromDate, DateTime? toDate, int pageSize, int pageNumber, string orderBy, string direction);
        Stream ConvertToCsvStream(IEnumerable<HmrSaltReport> saltReportEntities);
        public byte[] FillPdf(string templateName, Dictionary<string, string> data);
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
                    var saltReport = MapToEntity(dto);

                    await _context.HmrSaltReports.AddAsync(saltReport);
                    _unitOfWork.Commit();


                    if (dto.Sect4.Stockpiles != null && dto.Sect4.Stockpiles.Any())
                    {
                        var stockpiles = MapToStockpiles(dto.Sect4.Stockpiles);  // Map stockpiles using AutoMapper
                        foreach (var stockpile in stockpiles)
                        {
                            stockpile.SaltReportId = saltReport.SaltReportId;  // Set the foreign key
                        }
                        await _context.HmrSaltStockpiles.AddRangeAsync(stockpiles);
                        _unitOfWork.Commit();  // Commit stockpiles
                    }

                    if (dto.Sect7.VulnerableAreas != null && dto.Sect7.VulnerableAreas.Any())
                    {
                        var vulnareas = MapToVulnareas(dto.Sect7.VulnerableAreas);  // Map vulnareas using AutoMapper
                        foreach (var vulnArea in vulnareas)
                        {
                            vulnArea.SaltReportId = saltReport.SaltReportId;  // Set the foreign key
                        }
                        await _context.HmrSaltVulnAreas.AddRangeAsync(vulnareas);
                        _unitOfWork.Commit();  // Commit vulnerable areas
                    }

                    if (dto.Appendix != null)
                    {
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

        public List<HmrSaltVulnArea> MapToVulnareas(List<VulnareaDto> vulnareaDtos)
        {
            // Ensure the list is not null before attempting to map
            if (vulnareaDtos == null) throw new ArgumentNullException(nameof(vulnareaDtos));

            // Use AutoMapper to map the list of DTOs to the list of entities
            var vulnAreas = _mapper.Map<List<HmrSaltVulnArea>>(vulnareaDtos);

            return vulnAreas;
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

        public async Task<PagedDto<SaltReportDto>> GetSaltReportDtosAsync(string serviceAreas, DateTime? fromDate, DateTime? toDate, int pageSize, int pageNumber, string orderBy, string direction)
        {
            try
            {
                var saltReports = await _repository.GetPagedReportsAsync(serviceAreas, fromDate, toDate, pageSize, pageNumber, orderBy, direction).ConfigureAwait(false);
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
            using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    WriteCsvHeaders(writer); // Ensure headers are written correctly
                    RegisterCsvConverters(csv); // Ensure any custom converters are registered

                    csv.WriteRecords(saltReportEntities); // Write records

                    writer.Flush(); // Ensure writer is flushed to write all data to the stream
                }
                // Ensure no additional writes here that could affect the output
                WriteTotals(writer, saltReportEntities); // Write any totals or summaries if necessary
            }

            memoryStream.Position = 0; // Reset stream position to beginning

            return memoryStream;
        }

        private const int SaltReportCsvColumnCount = 145;

        private static string[] CreateCsvRow()
        {
            return Enumerable.Repeat(string.Empty, SaltReportCsvColumnCount).ToArray();
        }

        private static string[] CreateCsvRow(params (int Index, string Value)[] cells)
        {
            var row = CreateCsvRow();
            foreach (var cell in cells)
            {
                row[cell.Index] = cell.Value;
            }
            return row;
        }

        private static void WriteCsvRow(StreamWriter writer, string[] columns)
        {
            writer.WriteLine(string.Join(",", columns));
        }

        private static string CsvValue(object value)
        {
            return value?.ToString() ?? string.Empty;
        }

        private void WriteCsvHeaders(StreamWriter writer)
        {
            WriteCsvRow(writer, CreateCsvRow(
                (4, "1. Salt Management Plan"),
                (20, "2. Winter Ops"),
                (22, "3. Materials Applied"),
                (58, "4. Design & Operation at Road Salt Storage sites"),
                (89, "5. Salt Application"),
                (111, "6. Snow Disposal"),
                (125, "7. Management of Salt Vulnerable Areas")));

            WriteCsvRow(writer, CreateCsvRow(
                (4, "Salt Management Plan"),
                (7, "1.4 Training offered to:"),
                (12, "1.5 Objectives:"),
                (20, "(2.1 not used)"),
                (22, "3.1 Quantity of materials used:"),
                (50, "3.2 Multi-Chloride Liquids"),
                (58, "4.1"),
                (59, "4.2 Stockpile Conditions"),
                (67, "4.3 Good Housekeeping Practices"),
                (89, "5.1 Management of Equipment"),
                (96, "5.2 Weather Monitoring"),
                (103, "5.3 Maintenance Decision Support")));

            WriteCsvRow(writer, CreateCsvRow(
                (4, "1.1"),
                (5, "1.2"),
                (6, "1.3"),
                (7, "Managers"),
                (8, "Superv's"),
                (9, "Operators"),
                (10, "Mechanics"),
                (11, "Patrollers"),
                (12, "Storage Facilities"),
                (14, "Salt Application"),
                (16, "Snow Disposal"),
                (18, "Vulnerable Areas"),
                (20, "2.2 Total length"),
                (21, "2.3 # of days that salt was applied"),
                (22, "De-icers"),
                (27, "Treated Abrasives"),
                (32, "Pre-wetting liquid"),
                (38, "Pre-treatment liquid"),
                (44, "Direct Liquid Application"),
                (50, "Mix A"),
                (54, "Mix B"),
                (58, "Salt Storage"),
                (59, "Road Salts"),
                (63, "Treated Abrasive"),
                (67, "Good Housekeeping Practices"),
                (89, "# Vehicles used for salt application"),
                (96, "Sources of information"),
                (103, "Types of systems used to aid decision making"),
                (111, "6.1 Snow disposal sites"),
                (114, "6.2 Snow melters"),
                (116, "6.3 Meltwater management"),
                (125, "Salt Vulnerable Areas")));

            WriteCsvRow(writer, CreateCsvRow(
                (4, "developed"),
                (5, "reviewed"),
                (6, "updated"),
                (20, "of roads that are salted"),
                (21, "salt was applied"),
                (77, "Municipal sewer system"),
                (79, "Containment for removal"),
                (81, "Watercourse"),
                (83, "Other"),
                (94, "Regular Calibration?"),
                (111, "Perform snow disposal at designated site?"),
                (112, "Total sites"),
                (113, "Design capacity"),
                (114, "Use snow melters?"),
                (115, "% snow managed"),
                (116, "Meltwater disposal method used?"),
                (117, "Low permeability surface"),
                (119, "Retention pond"),
                (121, "Municipal sewer system"),
                (123, "Watercourse"),
                (125, "Inventory?"),
                (126, "Vulnerable areas?"),
                (127, "Action Plan?"),
                (128, "Mitigation Measures"),
                (129, "Monitoring?")));

            WriteCsvRow(writer, new[]
            {
                "SA", "Contact Name", "Number", "Email", "Y/N?", "Y/N?", "Y/N?", "Y/N?", "Y/N?", "Y/N?", "Y/N?", "Y/N?",
                "Identified", "Achieved", "Identified", "Achieved", "Identified", "Achieved", "Identified", "Achieved",
                "are salted?", "applied?", "NaCl", "MgCl2", "CaCl2", "Acetate", "HCOONa", "sand/etc", "NaCl", "MgCl2", "CaCl2", "HCOONa",
                "NaCl", "MgCl2", "CaCl2", "Acetate", "non-chloride", "HCOONa", "NaCl", "MgCl2", "CaCl2", "Acetate", "non-chloride", "HCOONa",
                "NaCl", "MgCl2", "CaCl2", "Acetate", "non-chloride", "HCOONa", "Litres", "NaCl %", "MgCl2 %", "CaCl2 %",
                "Litres", "NaCl %", "MgCl2 %", "CaCl2 %", "sites", "stockpiles", "# sites", "# sites", "# sites", "stockpiles", "# sites", "# sites", "# sites",
                "Y/N", "# sites", "Y/N", "# sites", "Y/N", "# sites", "Y/N", "# sites", "Y/N", "# sites", "Y/N", "# sites",
                "Y/N", "# sites", "Y/N", "# sites", "Y/N", "# sites", "Y/N", "# sites", "Y/N", "# sites",
                "of vehicles", "solid salt", "speed sensor", "pre-wetting", "liquid app", "Y/N", "#/Yr", "Y/N", "#", "Y/N", "Y/N", "#",
                "Y/N", "#", "Y/N", "#", "Y/N", "#", "Y/N", "#", "Y/N", "#", "Y/N", "#", "m3", "Y/N", "%", "Y/N",
                "%", "# sites", "%", "# sites", "%", "# sites", "%", "# sites", "Y/N", "Y/N", "Y/N", "Y/N", "Y/N",
                "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#"
            });
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
            var TotalSnowDisposalIdentified = reports.GetTotalIntValue(report => report.SnowDisposalIdentified);
            var TotalSnowDisposalAchieved = reports.GetTotalIntValue(report => report.SnowDisposalAchieved);
            var TotalVulnerableAreasIdentified = reports.GetTotalIntValue(report => report.VulnerableAreasIdentified);
            var TotalVulnerableAreasAchieved = reports.GetTotalIntValue(report => report.VulnerableAreasAchieved);
            var TotalRoadTotalLength = reports.GetTotalIntValue(report => report.RoadTotalLength);
            var TotalSaltTotalDays = reports.GetTotalIntValue(report => report.SaltTotalDays);
            var TotalDeicerNacl = reports.GetTotalDecimalValue(report => report.DeicerNacl);
            var TotalDeicerMgcl2 = reports.GetTotalDecimalValue(report => report.DeicerMgcl2);
            var TotalDeicerCacl2 = reports.GetTotalDecimalValue(report => report.DeicerCacl2);
            var TotalDeicerAcetate = reports.GetTotalDecimalValue(report => report.DeicerAcetate);
            var TotalDeicerSodiumFormate = reports.GetTotalDecimalValue(report => report.DeicerSodiumFormate);
            var TotalTreatedAbrasivesSandstoneDust = reports.GetTotalDecimalValue(report => report.TreatedAbrasivesSandstoneDust);
            var TotalTreatedAbrasivesNacl = reports.GetTotalDecimalValue(report => report.TreatedAbrasivesNacl);
            var TotalTreatedAbrasivesMgcl2 = reports.GetTotalDecimalValue(report => report.TreatedAbrasivesMgcl2);
            var TotalTreatedAbrasivesCacl2 = reports.GetTotalDecimalValue(report => report.TreatedAbrasivesCacl2);
            var TotalTreatedAbrasivesSodiumFormate = reports.GetTotalDecimalValue(report => report.TreatedAbrasivesSodiumFormate);
            var TotalPrewettingNacl = reports.GetTotalDecimalValue(report => report.PrewettingNacl);
            var TotalPrewettingMgcl2 = reports.GetTotalDecimalValue(report => report.PrewettingMgcl2);
            var TotalPrewettingCacl2 = reports.GetTotalDecimalValue(report => report.PrewettingCacl2);
            var TotalPrewettingAcetate = reports.GetTotalDecimalValue(report => report.PrewettingAcetate);
            var TotalPrewettingNonchloride = reports.GetTotalDecimalValue(report => report.PrewettingNonchloride);
            var TotalPrewettingSodiumFormate = reports.GetTotalDecimalValue(report => report.PrewettingSodiumFormate);
            var TotalPretreatmentNacl = reports.GetTotalDecimalValue(report => report.PretreatmentNacl);
            var TotalPretreatmentMgcl2 = reports.GetTotalDecimalValue(report => report.PretreatmentMgcl2);
            var TotalPretreatmentCacl2 = reports.GetTotalDecimalValue(report => report.PretreatmentCacl2);
            var TotalPretreatmentAcetate = reports.GetTotalDecimalValue(report => report.PretreatmentAcetate);
            var TotalPretreatmentNonchloride = reports.GetTotalDecimalValue(report => report.PretreatmentNonchloride);
            var TotalPretreatmentSodiumFormate = reports.GetTotalDecimalValue(report => report.PretreatmentSodiumFormate);
            var TotalAntiicingNacl = reports.GetTotalDecimalValue(report => report.AntiicingNacl);
            var TotalAntiicingMgcl2 = reports.GetTotalDecimalValue(report => report.AntiicingMgcl2);
            var TotalAntiicingCacl2 = reports.GetTotalDecimalValue(report => report.AntiicingCacl2);
            var TotalAntiicingAcetate = reports.GetTotalDecimalValue(report => report.AntiicingAcetate);
            var TotalAntiicingNonchloride = reports.GetTotalDecimalValue(report => report.AntiicingNonchloride);
            var TotalAntiicingSodiumFormate = reports.GetTotalDecimalValue(report => report.AntiicingSodiumFormate);
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
            var TotalSnowDisposalSiteCapacity = reports.GetTotalIntValue(report => report.SnowDisposalSiteCapacity);
            var TotalSnowMeltersUsed = reports.GetTotalBoolValue(report => report.SnowMeltersUsed);
            var TotalSnowMeltersSnowPercentage = reports.GetTotalDecimalValue(report => report.SnowMeltersSnowPercentage);
            var TotalMeltwaterDisposalMethodUsed = reports.GetTotalBoolValue(report => report.MeltwaterDisposalMethodUsed);
            var TotalSnowLowPermeabilitySurfacePercentage = reports.GetTotalDecimalValue(report => report.SnowLowPermeabilitySurfacePercentage);
            var TotalSnowLowPermeabilitySurfaceSites = reports.GetTotalIntValue(report => report.SnowLowPermeabilitySurfaceSites);
            var TotalMeltwaterRetentionPondPercentage = reports.GetTotalDecimalValue(report => report.MeltwaterRetentionPondPercentage);
            var TotalMeltwaterRetentionPondSites = reports.GetTotalIntValue(report => report.MeltwaterRetentionPondSites);
            var TotalMeltwaterMunicipalSewerPercentage = reports.GetTotalDecimalValue(report => report.MeltwaterMunicipalSewerPercentage);
            var TotalMeltwaterMunicipalSewerSites = reports.GetTotalIntValue(report => report.MeltwaterMunicipalSewerSites);
            var TotalMeltwaterWatercoursePercentage = reports.GetTotalDecimalValue(report => report.MeltwaterWatercoursePercentage);
            var TotalMeltwaterWatercourseSites = reports.GetTotalIntValue(report => report.MeltwaterWatercourseSites);
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

            var totalRow = CreateCsvRow();
            totalRow[0] = "Total / Total Yes";
            totalRow[4] = CsvValue(TotalPlanDeveloped);
            totalRow[5] = CsvValue(TotalPlanReviewed);
            totalRow[6] = CsvValue(TotalPlanUpdated);
            totalRow[7] = CsvValue(TotalManagerTraining);
            totalRow[8] = CsvValue(TotalSupervisorTraining);
            totalRow[9] = CsvValue(TotalOperatorTraining);
            totalRow[10] = CsvValue(TotalMechanicalTraining);
            totalRow[11] = CsvValue(TotalPatrollerTraining);
            totalRow[12] = CsvValue(TotalMaterialStorageIdentified);
            totalRow[13] = CsvValue(TotalMaterialStorageAchieved);
            totalRow[14] = CsvValue(TotalSaltApplicationIdentified);
            totalRow[15] = CsvValue(TotalSaltApplicationAchieved);
            totalRow[16] = CsvValue(TotalSnowDisposalIdentified);
            totalRow[17] = CsvValue(TotalSnowDisposalAchieved);
            totalRow[18] = CsvValue(TotalVulnerableAreasIdentified);
            totalRow[19] = CsvValue(TotalVulnerableAreasAchieved);
            totalRow[20] = CsvValue(TotalRoadTotalLength);
            totalRow[21] = CsvValue(TotalSaltTotalDays);
            totalRow[22] = CsvValue(TotalDeicerNacl);
            totalRow[23] = CsvValue(TotalDeicerMgcl2);
            totalRow[24] = CsvValue(TotalDeicerCacl2);
            totalRow[25] = CsvValue(TotalDeicerAcetate);
            totalRow[26] = CsvValue(TotalDeicerSodiumFormate);
            totalRow[27] = CsvValue(TotalTreatedAbrasivesSandstoneDust);
            totalRow[28] = CsvValue(TotalTreatedAbrasivesNacl);
            totalRow[29] = CsvValue(TotalTreatedAbrasivesMgcl2);
            totalRow[30] = CsvValue(TotalTreatedAbrasivesCacl2);
            totalRow[31] = CsvValue(TotalTreatedAbrasivesSodiumFormate);
            totalRow[32] = CsvValue(TotalPrewettingNacl);
            totalRow[33] = CsvValue(TotalPrewettingMgcl2);
            totalRow[34] = CsvValue(TotalPrewettingCacl2);
            totalRow[35] = CsvValue(TotalPrewettingAcetate);
            totalRow[36] = CsvValue(TotalPrewettingNonchloride);
            totalRow[37] = CsvValue(TotalPrewettingSodiumFormate);
            totalRow[38] = CsvValue(TotalPretreatmentNacl);
            totalRow[39] = CsvValue(TotalPretreatmentMgcl2);
            totalRow[40] = CsvValue(TotalPretreatmentCacl2);
            totalRow[41] = CsvValue(TotalPretreatmentAcetate);
            totalRow[42] = CsvValue(TotalPretreatmentNonchloride);
            totalRow[43] = CsvValue(TotalPretreatmentSodiumFormate);
            totalRow[44] = CsvValue(TotalAntiicingNacl);
            totalRow[45] = CsvValue(TotalAntiicingMgcl2);
            totalRow[46] = CsvValue(TotalAntiicingCacl2);
            totalRow[47] = CsvValue(TotalAntiicingAcetate);
            totalRow[48] = CsvValue(TotalAntiicingNonchloride);
            totalRow[49] = CsvValue(TotalAntiicingSodiumFormate);
            totalRow[50] = CsvValue(TotalMultichlorideALitres);
            totalRow[51] = CsvValue(TotalMultichlorideANaclPercentage);
            totalRow[52] = CsvValue(TotalMultichlorideAMgcl2Percentage);
            totalRow[53] = CsvValue(TotalMultichlorideACacl2Percentage);
            totalRow[54] = CsvValue(TotalMultichlorideBLitres);
            totalRow[55] = CsvValue(TotalMultichlorideBNaclPercentage);
            totalRow[56] = CsvValue(TotalMultichlorideBMgcl2Percentage);
            totalRow[57] = CsvValue(TotalMultichlorideBCacl2Percentage);
            totalRow[58] = CsvValue(TotalSaltStorageSitesTotal);
            totalRow[59] = CsvValue(TotalRoadSaltStockpilesTotal);
            totalRow[60] = CsvValue(TotalRoadSaltOnImpermeableSurface);
            totalRow[61] = CsvValue(TotalRoadSaltUnderPermanentRoof);
            totalRow[62] = CsvValue(TotalRoadSaltUnderTarp);
            totalRow[63] = CsvValue(TotalTreatedAbrasivesStockpilesTotal);
            totalRow[64] = CsvValue(TotalTreatedAbrasivesOnImpermeableSurface);
            totalRow[65] = CsvValue(TotalTreatedAbrasivesUnderPermanentRoof);
            totalRow[66] = CsvValue(TotalTreatedAbrasivesUnderTarp);
            totalRow[67] = CsvValue(TotalAllMaterialsHandledPlan);
            totalRow[68] = CsvValue(TotalAllMaterialsHandledSites);
            totalRow[69] = CsvValue(TotalEquipmentPreventsOverloadingPlan);
            totalRow[70] = CsvValue(TotalEquipmentPreventsOverloadingSites);
            totalRow[71] = CsvValue(TotalWastewaterSystemPlan);
            totalRow[72] = CsvValue(TotalWastewaterSystemSites);
            totalRow[73] = CsvValue(TotalControlDiversionExternalWatersPlan);
            totalRow[74] = CsvValue(TotalControlDiversionExternalWatersSites);
            totalRow[75] = CsvValue(TotalDrainageCollectionSystemPlan);
            totalRow[76] = CsvValue(TotalDrainageCollectionSystemSites);
            totalRow[77] = CsvValue(TotalMunicipalSewerSystemPlan);
            totalRow[78] = CsvValue(TotalMunicipalSewerSystemSites);
            totalRow[79] = CsvValue(TotalRemovalContainmentPlan);
            totalRow[80] = CsvValue(TotalRemovalContainmentSites);
            totalRow[81] = CsvValue(TotalWatercoursePlan);
            totalRow[82] = CsvValue(TotalWatercourseSites);
            totalRow[83] = CsvValue(TotalOtherDischargePointPlan);
            totalRow[84] = CsvValue(TotalOtherDischargePointSites);
            totalRow[85] = CsvValue(TotalOngoingCleanupPlan);
            totalRow[86] = CsvValue(TotalOngoingCleanupSites);
            totalRow[87] = CsvValue(TotalRiskManagementPlanPlan);
            totalRow[88] = CsvValue(TotalRiskManagementPlanSites);
            totalRow[89] = CsvValue(TotalNumberOfVehicles);
            totalRow[90] = CsvValue(TotalVehiclesForSaltApplication);
            totalRow[91] = CsvValue(TotalVehiclesWithConveyors);
            totalRow[92] = CsvValue(TotalVehiclesWithPreWettingEquipment);
            totalRow[93] = CsvValue(TotalVehiclesForDLA);
            totalRow[94] = CsvValue(TotalRegularCalibration);
            totalRow[95] = CsvValue(TotalRegularCalibrationTotal);
            totalRow[96] = CsvValue(TotalInfraredThermometerRelied);
            totalRow[97] = CsvValue(TotalInfraredThermometerTotal);
            totalRow[98] = CsvValue(TotalMeteorologicalServiceRelied);
            totalRow[99] = CsvValue(TotalFixedRWISStationsRelied);
            totalRow[100] = CsvValue(TotalFixedRWISStationsTotal);
            totalRow[101] = CsvValue(TotalMobileRWISMountedRelied);
            totalRow[102] = CsvValue(TotalMobileRWISMountedTotal);
            totalRow[103] = CsvValue(TotalAVLRelied);
            totalRow[104] = CsvValue(TotalAVLTotal);
            totalRow[105] = CsvValue(TotalSaltApplicationRatesRelied);
            totalRow[106] = CsvValue(TotalSaltApplicationRatesTotal);
            totalRow[107] = CsvValue(TotalApplicationRateChartRelied);
            totalRow[108] = CsvValue(TotalApplicationRateChartTotal);
            totalRow[109] = CsvValue(TotalTestingMDSSRelied);
            totalRow[110] = CsvValue(TotalTestingSMDSTotal);
            totalRow[111] = CsvValue(TotalSnowDisposalSiteUsed);
            totalRow[112] = CsvValue(TotalSnowDisposalSiteTotal);
            totalRow[113] = CsvValue(TotalSnowDisposalSiteCapacity);
            totalRow[114] = CsvValue(TotalSnowMeltersUsed);
            totalRow[115] = CsvValue(TotalSnowMeltersSnowPercentage);
            totalRow[116] = CsvValue(TotalMeltwaterDisposalMethodUsed);
            totalRow[117] = CsvValue(TotalSnowLowPermeabilitySurfacePercentage);
            totalRow[118] = CsvValue(TotalSnowLowPermeabilitySurfaceSites);
            totalRow[119] = CsvValue(TotalMeltwaterRetentionPondPercentage);
            totalRow[120] = CsvValue(TotalMeltwaterRetentionPondSites);
            totalRow[121] = CsvValue(TotalMeltwaterMunicipalSewerPercentage);
            totalRow[122] = CsvValue(TotalMeltwaterMunicipalSewerSites);
            totalRow[123] = CsvValue(TotalMeltwaterWatercoursePercentage);
            totalRow[124] = CsvValue(TotalMeltwaterWatercourseSites);
            totalRow[125] = CsvValue(TotalCompletedInventory);
            totalRow[126] = CsvValue(TotalSetVulnerableAreas);
            totalRow[127] = CsvValue(TotalActionPlanPrepared);
            totalRow[128] = CsvValue(TotalProtectionMeasuresImplemented);
            totalRow[129] = CsvValue(TotalEnvironmentalMonitoringConducted);
            totalRow[130] = CsvValue(TotalDrinkingWaterAreasIdentified);
            totalRow[131] = CsvValue(TotalDrinkingWaterAreasWithProtection);
            totalRow[132] = CsvValue(TotalDrinkingWaterAreasWithChloride);
            totalRow[133] = CsvValue(TotalAquaticLifeAreasIdentified);
            totalRow[134] = CsvValue(TotalAquaticLifeAreasWithProtection);
            totalRow[135] = CsvValue(TotalAquaticLifeAreasWithChloride);
            totalRow[136] = CsvValue(TotalWetlandsAreasIdentified);
            totalRow[137] = CsvValue(TotalWetlandsAreasWithProtection);
            totalRow[138] = CsvValue(TotalWetlandsAreasWithChloride);
            totalRow[139] = CsvValue(TotalDelimitedAreasAreasIdentified);
            totalRow[140] = CsvValue(TotalDelimitedAreasAreasWithProtection);
            totalRow[141] = CsvValue(TotalDelimitedAreasAreasWithChloride);
            totalRow[142] = CsvValue(TotalValuedLandsAreasIdentified);
            totalRow[143] = CsvValue(TotalValuedLandsAreasWithProtection);
            totalRow[144] = CsvValue(TotalValuedLandsAreasWithChloride);
            WriteCsvRow(writer, totalRow);

            var totalNoRow = CreateCsvRow();
            totalNoRow[0] = "Total No (treats empties as No)";
            totalNoRow[4] = CsvValue(totalReports - TotalPlanDeveloped);
            totalNoRow[5] = CsvValue(totalReports - TotalPlanReviewed);
            totalNoRow[6] = CsvValue(totalReports - TotalPlanUpdated);
            totalNoRow[7] = CsvValue(totalReports - TotalManagerTraining);
            totalNoRow[8] = CsvValue(totalReports - TotalSupervisorTraining);
            totalNoRow[9] = CsvValue(totalReports - TotalOperatorTraining);
            totalNoRow[10] = CsvValue(totalReports - TotalMechanicalTraining);
            totalNoRow[11] = CsvValue(totalReports - TotalPatrollerTraining);
            totalNoRow[67] = CsvValue(totalReports - TotalAllMaterialsHandledPlan);
            totalNoRow[69] = CsvValue(totalReports - TotalEquipmentPreventsOverloadingPlan);
            totalNoRow[71] = CsvValue(totalReports - TotalWastewaterSystemPlan);
            totalNoRow[73] = CsvValue(totalReports - TotalControlDiversionExternalWatersPlan);
            totalNoRow[75] = CsvValue(totalReports - TotalDrainageCollectionSystemPlan);
            totalNoRow[77] = CsvValue(totalReports - TotalMunicipalSewerSystemPlan);
            totalNoRow[79] = CsvValue(totalReports - TotalRemovalContainmentPlan);
            totalNoRow[81] = CsvValue(totalReports - TotalWatercoursePlan);
            totalNoRow[83] = CsvValue(totalReports - TotalOtherDischargePointPlan);
            totalNoRow[85] = CsvValue(totalReports - TotalOngoingCleanupPlan);
            totalNoRow[87] = CsvValue(totalReports - TotalRiskManagementPlanPlan);
            totalNoRow[94] = CsvValue(totalReports - TotalRegularCalibration);
            totalNoRow[96] = CsvValue(totalReports - TotalInfraredThermometerRelied);
            totalNoRow[98] = CsvValue(totalReports - TotalMeteorologicalServiceRelied);
            totalNoRow[99] = CsvValue(totalReports - TotalFixedRWISStationsRelied);
            totalNoRow[101] = CsvValue(totalReports - TotalMobileRWISMountedRelied);
            totalNoRow[103] = CsvValue(totalReports - TotalAVLRelied);
            totalNoRow[105] = CsvValue(totalReports - TotalSaltApplicationRatesRelied);
            totalNoRow[107] = CsvValue(totalReports - TotalApplicationRateChartRelied);
            totalNoRow[109] = CsvValue(totalReports - TotalTestingMDSSRelied);
            totalNoRow[111] = CsvValue(totalReports - TotalSnowDisposalSiteUsed);
            totalNoRow[114] = CsvValue(totalReports - TotalSnowMeltersUsed);
            totalNoRow[116] = CsvValue(totalReports - TotalMeltwaterDisposalMethodUsed);
            totalNoRow[125] = CsvValue(totalReports - TotalCompletedInventory);
            totalNoRow[126] = CsvValue(totalReports - TotalSetVulnerableAreas);
            totalNoRow[127] = CsvValue(totalReports - TotalActionPlanPrepared);
            totalNoRow[128] = CsvValue(totalReports - TotalProtectionMeasuresImplemented);
            totalNoRow[129] = CsvValue(totalReports - TotalEnvironmentalMonitoringConducted);
            WriteCsvRow(writer, totalNoRow);
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

        public Dictionary<string, string> ExtractFields(string templateName)
        {
            var pdfBytes = _repository.GetPdfTemplate(templateName);

            using (var pdfReader = new PdfReader(new MemoryStream(pdfBytes)))
            using (var pdfDoc = new PdfDocument(pdfReader))
            {
                var form = PdfAcroForm.GetAcroForm(pdfDoc, false);
                var fields = form?.GetAllFormFields();

                if (fields == null) return new Dictionary<string, string>();

                return fields.ToDictionary(field => field.Key, field => field.Value.GetValueAsString());
            }
        }

        public byte[] FillPdf(string resourceName, Dictionary<string, string> data)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = $"Hmcr.Domain.PdfTemplates.{resourceName}";

            using var resourceStream = assembly.GetManifestResourceStream(resourcePath)
                ?? throw new FileNotFoundException($"Resource '{resourcePath}' not found.");
            
            using var pdfReader = new PdfReader(resourceStream);
            using var memoryStream = new MemoryStream();
            using var pdfWriter = new PdfWriter(memoryStream);
            using var pdfDoc = new PdfDocument(pdfReader, pdfWriter);

            var form = PdfAcroForm.GetAcroForm(pdfDoc, true);

            foreach (var fieldEntry in data)
            {
                var field = form.GetField(fieldEntry.Key);

                if (field != null)
                {
                    field.SetValue(fieldEntry.Value);
                    field.SetFontSize(10);
                    field.SetJustification(TextAlignment.CENTER);
                }
                else
                {
                    Console.WriteLine($"Field '{fieldEntry.Key}' not found in the PDF.");
                }
            }
            form.FlattenFields();
            pdfDoc.Close();

            // Return the filled PDF as byte array
            return memoryStream.ToArray();
        }
    }
}
