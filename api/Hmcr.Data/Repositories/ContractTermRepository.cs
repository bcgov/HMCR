using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.ContractTerm;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IContractTermRepository 
    {
        Task<ContractTermDto> GetContractTerm(decimal serviceAreaNumber, DateTime date);
    }
    public class ContractTermRepository : HmcrRepositoryBase<HmrContractTerm>, IContractTermRepository
    {
        public ContractTermRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<ContractTermDto> GetContractTerm(decimal serviceAreaNumber, DateTime date)
        {
            var contract = await GetFirstOrDefaultAsync<ContractTermDto>(x => x.ServiceAreaNumber == serviceAreaNumber && x.StartDate <= date && x.EndDate > date);

            return contract;
        }
    }
}
