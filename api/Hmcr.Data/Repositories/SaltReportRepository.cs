using System.Collections.Generic;
using Hmcr.Data.Database.Entities;
using Hmcr.Model.Dtos.SaltReport;
using Hmcr.Model.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Hmcr.Data.Repositories.Base;
using AutoMapper;

namespace Hmcr.Data.Repositories
{
    public interface ISaltReportRepository
    {
        Task<HmrSaltReport> AddAsync(HmrSaltReport saltReport, List<HmrSaltStockpile> stockpiles);
    }

    public class SaltReportRepository : HmcrRepositoryBase<HmrSaltReport>, ISaltReportRepository
    {
        private readonly AppDbContext _context;

        public SaltReportRepository(AppDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            _context = context;
        }

        public async Task<HmrSaltReport> AddAsync(HmrSaltReport saltReport, List<HmrSaltStockpile> stockpiles)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Add to table HMR_SALT_REPORT
                    _context.HmrSaltReports.Add(saltReport);
                    // Save changes to generate HmrSaltReport's ID
                    _context.SaveChanges();

                    // Associate stockiples with the report
                    foreach (var stockpile in stockpiles)
                    {
                        stockpile.SaltReportId = saltReport.SaltReportId;
                        _context.HmrSaltStockpiles.Add(stockpile);
                    }
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
    }
}