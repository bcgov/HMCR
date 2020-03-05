using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IContractTermRepository 
    {
        Task<decimal> GetContractPartyId(decimal serviceAreaNumber, DateTime date);
    }
    public class ContractTermRepository : HmcrRepositoryBase<HmrContractTerm>, IContractTermRepository
    {
        public ContractTermRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<decimal> GetContractPartyId(decimal serviceAreaNumber, DateTime date)
        {
            var contract = await DbSet.FirstOrDefaultAsync(x => x.ServiceAreaNumber == serviceAreaNumber && x.StartDate <= date && x.EndDate > date);

            return contract == null ? 0 : contract.PartyId ?? 0;
        }
    }
}
