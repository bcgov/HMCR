using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.SaltReport;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.SaltReport;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;

namespace Hmcr.Domain.Services
{
    public interface ISaltReportService
    {
        Task<HmrSaltReport> CreateSaltReportAsync(SaltReportDto dto);
    }

    public class SaltReportService : ISaltReportService
    {
        private ISaltReportRepository _repository;
        private IMapper _mapper;

        public SaltReportService(ISaltReportRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<HmrSaltReport> CreateSaltReportAsync(SaltReportDto dto)
        {
            // TODO: DTO VALIDATION
            // CODE HERE...
            // ValidateDto(dto)

            var saltReportEntity = MapDtoToEntity(dto);
            var stockPileEntity = dto.Sect4.Stockpiles?.Select(MapStockpileDtoToEntity).ToList();

            // TODO: ADD ADDITIONAL BUSINESS LOGIC
            // CODE HERE...
            // ProcessBusinessRules(saltReport)

            return await _repository.AddAsync(saltReportEntity, stockPileEntity);
        }

        private HmrSaltReport MapDtoToEntity(SaltReportDto dto)
        {
            var entity = _mapper.Map<HmrSaltReport>(dto);

            return entity;
        }

        private HmrSaltStockpile MapStockpileDtoToEntity(StockpileDto dto)
        {
            var entity = _mapper.Map<HmrSaltStockpile>(dto);

            return entity;
        }
        public void ValidateDto(SaltReportDto dto)
        {
            // Validation Logic
        }

        public void ProcessBusinessRules(HmrSaltReport saltReport)
        {
            // Business Logic
        }
    }
}