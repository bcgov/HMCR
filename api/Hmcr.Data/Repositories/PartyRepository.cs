using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IPartyRepository : IHmcrRepositoryBase<HmrParty>
    {
        HmrParty GetPartyEntityByGuid(Guid? guid);
    }

    public class PartyRepository : HmcrRepositoryBase<HmrParty>, IPartyRepository
    {
        public PartyRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public HmrParty GetPartyEntityByGuid(Guid? guid)
        {
            if (guid == null) 
                throw new ArgumentNullException("GetPartyEntityByGuidAsync guid null");

            return DbSet.FirstOrDefault(p => p.BusinessGuid == guid);
        }

    }
}
