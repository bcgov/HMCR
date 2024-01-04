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
        Task<HmrSaltReport> AddAsync(HmrSaltReport saltReport);
    }

    public class SaltReportRepository : HmcrRepositoryBase<HmrSaltReport>, ISaltReportRepository
    {
        private readonly AppDbContext _context;

        public SaltReportRepository(AppDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            _context = context;
        }

        public async Task<HmrSaltReport> AddAsync(HmrSaltReport saltReport)
        {
            _context.HmrSaltReports.Add(saltReport);
            // var entry = _context.Entry(saltReport);
            // Console.WriteLine($"Entity State: {entry.State}");

            Console.WriteLine(_context);
            _context.SaveChanges();

            // var affectedRows = await _context.SaveChangesAsync();
            return saltReport;
        }
    }
}