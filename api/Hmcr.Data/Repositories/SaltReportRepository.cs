using System.Collections.Generic;
using Hmcr.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Hmcr.Data.Repositories.Base;
using AutoMapper;

namespace Hmcr.Data.Repositories
{
    public interface ISaltReportRepository : IHmcrRepositoryBase<HmrSaltReport>
    {
        Task<HmrSaltReport> AddReportAsync(HmrSaltReport saltReport);
        Task<IEnumerable<HmrSaltReport>> GetAllReportsAsync();
        Task<HmrSaltReport> GetReportByIdAsync(int saltReportId);
        Task<IEnumerable<HmrSaltReport>> GetReportsAsync(string serviceAreas, DateTime? fromDate, DateTime? toDate, string cql_filter);
    }

    public class SaltReportRepository : HmcrRepositoryBase<HmrSaltReport>, ISaltReportRepository
    {
        private readonly AppDbContext _context;

        public SaltReportRepository(AppDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<HmrSaltReport> AddReportAsync(HmrSaltReport saltReport)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Add to table HMR_SALT_REPORT
                    await _context.HmrSaltReports.AddAsync(saltReport);
                    // Save changes to generate HmrSaltReport's ID
                    _context.SaveChanges();

                    // Commit the transaction if everything is successful
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // An exception occured, roll back the transaction
                    transaction.Rollback();
                    throw;
                }
            }

            return saltReport;
        }

        public async Task<IEnumerable<HmrSaltReport>> GetAllReportsAsync()
        {
            return await _context.HmrSaltReports
                .Include(report => report.Stockpiles)
                .ToListAsync();
        }

        public async Task<IEnumerable<HmrSaltReport>> GetReportsAsync(string serviceAreas, DateTime? fromDate, DateTime? toDate, string cql_filter)
        {
            var query = _context.HmrSaltReports.AsQueryable();

            // Apply service area filter if serviceAreas is not null or empty.
            if (!string.IsNullOrWhiteSpace(serviceAreas))
            {
                var serviceAreaList = serviceAreas.Split(',').Select(decimal.Parse).ToList();
                query = query.Where(report => serviceAreaList.Contains(report.ServiceArea));
            }

            // Apply date range filter if fromDate and toDate are provided.
            if (fromDate.HasValue && toDate.HasValue)
            {
                // Increase toDate by one day because initial toDate value starts at midnight
                var adjustedToDate = toDate.Value.AddDays(2);
                query = query.Where(report => report.AppCreateTimestamp >= fromDate.Value && report.AppCreateTimestamp <= adjustedToDate);
                query = query.Include(report => report.Stockpiles);

                return await query.ToListAsync();
            }

            // Nothing is provided; return all reports
            return await GetAllReportsAsync();

        }

        public async Task<HmrSaltReport> GetReportByIdAsync(int saltReportId)
        {
            return await _context.HmrSaltReports.FirstOrDefaultAsync(r => r.SaltReportId == saltReportId);
        }
    }
}