using System.Collections.Generic;
using Hmcr.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Hmcr.Data.Repositories.Base;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using Hmcr.Model.Dtos.SaltReport;
using Hmcr.Model.Dtos;
using Microsoft.Identity.Client;

namespace Hmcr.Data.Repositories
{
    public interface ISaltReportRepository : IHmcrRepositoryBase<HmrSaltReport>
    {
        Task AddReportAsync(HmrSaltReport saltReport);
        Task<IEnumerable<HmrSaltReport>> GetAllReportsAsync();
        Task<HmrSaltReport> GetReportByIdAsync(int saltReportId);
        Task<IEnumerable<HmrSaltReport>> GetReportsAsync(string serviceAreas, DateTime? fromDate, DateTime? toDate);
        Task<PagedDto<SaltReportDto>> GetPagedReportsAsync(string serviceAreas, DateTime? fromDate, DateTime? toDate, int pageSize, int pageNumber);
        Task AddStockpilesAsync(IEnumerable<HmrSaltStockpile> stockpiles);
        Task AddAppendixAsync(HmrSaltReportAppendix appendix);
    }

    public class SaltReportRepository : HmcrRepositoryBase<HmrSaltReport>, ISaltReportRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SaltReportRepository> _logger;

        public SaltReportRepository(AppDbContext context, IMapper mapper, ILogger<SaltReportRepository> logger)
            : base(context, mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddReportAsync(HmrSaltReport saltReport)
        {
            await DbSet.AddAsync(saltReport);
        }

        public async Task AddStockpilesAsync(IEnumerable<HmrSaltStockpile> stockpiles)
        {
            await _context.HmrSaltStockpiles.AddRangeAsync(stockpiles);
        }

        public async Task AddAppendixAsync(HmrSaltReportAppendix appendix)
        {
            await _context.HmrSaltReportAppendixes.AddAsync(appendix);
        }

        public async Task<IEnumerable<HmrSaltReport>> GetAllReportsAsync()
        {
            return await _context.HmrSaltReports
                .Include(report => report.Stockpiles)
                .ToListAsync();
        }

        public async Task<PagedDto<SaltReportDto>> GetPagedReportsAsync(string serviceAreas, DateTime? fromDate, DateTime? toDate, int pageSize = 5, int pageNumber = 0)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
            {
                throw new ArgumentException("Starting date cannot be later than last date");
            }

            _logger.LogInformation("Obtaining as queryable");

            IQueryable<HmrSaltReport> query = DbSet.AsNoTracking();

            // Apply service area filter
            if (!string.IsNullOrWhiteSpace(serviceAreas))
            {
                var serviceAreaList = serviceAreas.Split(',')
                                                   .Select(s => decimal.TryParse(s, out var result) ? result : -1)
                                                   .Where(n => n != -1);
                _logger.LogInformation($"Parsed service areas: {string.Join(",", serviceAreaList)}");
                query = query.Where(report => serviceAreaList.Contains(report.ServiceArea));
            }

            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(report => report.AppCreateTimestamp >= fromDate && report.AppCreateTimestamp < toDate.Value.AddDays(1));
            }

            int totalCount = await query.CountAsync();

            // Mapping to DTOs
            var reports = await query.Select(report => new SaltReportDto
            {
                SaltReportId = report.SaltReportId,
                AppCreateTimestamp = report.AppCreateTimestamp,
                ServiceArea = report.ServiceArea,
                ContactName = report.ContactName
            })
            .OrderBy(report => report.AppCreateTimestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            _logger.LogInformation($"Retrieved {reports.Count} reports");

            return new PagedDto<SaltReportDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                SourceList = reports
            };
        }

        public async Task<IEnumerable<HmrSaltReport>> GetReportsAsync(string serviceAreas, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                {
                    throw new ArgumentException("Starting date cannot be later than last date");
                }
                _logger.LogInformation($"Obtaining as queryable");

                var query = DbSet.AsNoTracking();

                _logger.LogInformation($"Received service areas: {serviceAreas}");

                // Apply service area filter if serviceAreas is not null or empty.
                if (!string.IsNullOrWhiteSpace(serviceAreas))
                {
                    var serviceAreaList = serviceAreas.Split(',')
                                                       .Select(s => decimal.TryParse(s, out var result) ? result : -1) // Or whatever default value is appropriate
                                                       .Where(n => n != -1) // Filter out invalid parsed values
                                                       .ToList();
                    _logger.LogInformation($"Parsed service areas: {string.Join(",", serviceAreaList)}");
                    query = query.Where(report => serviceAreaList.Contains(report.ServiceArea));
                    _logger.LogInformation($"Successfully obtained service Area Filter");
                }


                _logger.LogInformation($"Applying date range filter");

                // Apply date range filter if fromDate and toDate are provided.
                if (fromDate.HasValue && toDate.HasValue)
                {
                    var adjustedToDate = toDate.Value.AddDays(1);
                    query = query.Where(report => report.AppCreateTimestamp >= fromDate && report.AppCreateTimestamp <= adjustedToDate);
                    _logger.LogInformation("Date range filter applied");
                }
                _logger.LogInformation("Generated SQL query: " + query.ToQueryString());

                var reports = await query
                    .Include(report => report.Stockpiles)
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {reports.Count} reports");
                return reports;

            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument.");
                throw;
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Database exception occured in GetReportsAsync");
                throw new RepositoryException("An error occured while retrieving reports.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetReportsAsync");
                throw; // Re-throw exception to propagate it up the call stack
            }

        }

        public class RepositoryException : Exception
        {
            public RepositoryException() { }

            public RepositoryException(string message) : base(message) { }

            public RepositoryException(string message, Exception innerException) : base(message, innerException) { }
        }

        public async Task<HmrSaltReport> GetReportByIdAsync(int saltReportId)
        {
            return await _context.HmrSaltReports.FirstOrDefaultAsync(r => r.SaltReportId == saltReportId);
        }
    }
}