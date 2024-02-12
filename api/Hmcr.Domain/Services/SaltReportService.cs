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

namespace Hmcr.Domain.Services
{
    public interface ISaltReportService
    {
        Task<HmrSaltReport> CreateReportAsync(SaltReportDto dto);
        Task<IEnumerable<SaltReportDto>> GetSaltReportDtosAsync();
        Task<SaltReportDto> GetSaltReportByIdAsync(int saltReportId);
        Task<IEnumerable<HmrSaltReport>> GetSaltReportEntitiesAsync(string serviceAreas, DateTime fromDate, DateTime toDate, string cql_filter);
        Stream ConvertToCsvStream(IEnumerable<HmrSaltReport> saltReportEntities);
    }

    public class SaltReportService : ISaltReportService
    {
        private readonly ISaltReportRepository _repository;
        private readonly IMapper _mapper;

        public SaltReportService(ISaltReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HmrSaltReport> CreateReportAsync(SaltReportDto dto)
        {
            // TODO: DTO VALIDATION
            // CODE HERE...
            // ValidateDto(dto)

            var saltReport = MapToEntity(dto);

            // TODO: ADD ADDITIONAL BUSINESS LOGIC
            // CODE HERE...
            // ProcessBusinessRules(saltReport)

            return await _repository.AddReportAsync(saltReport);
        }

        private HmrSaltReport MapToEntity(SaltReportDto dto) => _mapper.Map<HmrSaltReport>(dto);

        public void ValidateDto(SaltReportDto dto)
        {
            // Validation Logic
        }

        public void ProcessBusinessRules(HmrSaltReport saltReport)
        {
            // Business Logic
        }

        public async Task<IEnumerable<SaltReportDto>> GetSaltReportDtosAsync()
        {
            var saltReportEntities = await _repository.GetAllReportsAsync();

            return _mapper.Map<IEnumerable<SaltReportDto>>(saltReportEntities);
        }

        public async Task<IEnumerable<HmrSaltReport>> GetSaltReportEntitiesAsync(string serviceAreas, DateTime fromDate, DateTime toDate, string cql_filter)
        {
            return await _repository.GetReportsAsync(serviceAreas, fromDate, toDate, cql_filter);
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

        private void RegisterCsvConverters(CsvWriter csv)
        {
            // Add custom boolean converter
            csv.Configuration.TypeConverterCache.AddConverter<bool>(new BooleanYesNoConverter());
            csv.Configuration.TypeConverterCache.AddConverter<bool?>(new BooleanYesNoConverter()); // For nullable booleans

            // Register the mapping
            csv.Configuration.RegisterClassMap<SaltReportMap>();
            csv.Configuration.HasHeaderRecord = false;
        }

    }
}