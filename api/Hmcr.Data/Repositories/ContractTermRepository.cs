using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IContractTermRepository 
    {
        Task<bool> HasContractTermAsync(decimal serviceAreaNumber, DateTime date);
    }
    public class ContractTermRepository : HmcrRepositoryBase<HmrContractTerm>, IContractTermRepository
    {
        public ContractTermRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<bool> HasContractTermAsync(decimal serviceAreaNumber, DateTime date)
        {
            return await DbSet.AnyAsync(x => x.ServiceAreaNumber == serviceAreaNumber && x.StartDate <= date && x.EndDate > date);
        }
    }
}
