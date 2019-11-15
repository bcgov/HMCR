using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IPartyRepository : IHmcrRepositoryBase<HmrParty>
    {
        Task<HmrParty> GetPartyEntityByGuidAsync(Guid? guid);
    }

    public class PartyRepository : HmcrRepositoryBase<HmrParty>, IPartyRepository
    {
        public PartyRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<HmrParty> GetPartyEntityByGuidAsync(Guid? guid)
        {
            if (guid == null) 
                throw new ArgumentNullException("GetPartyEntityByGuidAsync guid null");

            return await DbSet.FirstOrDefaultAsync(p => p.BusinessGuid == guid);
        }

    }
}
