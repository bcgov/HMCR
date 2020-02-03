using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.LocationCode;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
   
    public interface ILocationCodeDropDownRepository
    {
        Task<IEnumerable<LocationCodeDropDownDto>> GetLocationCodes();
    }
    
    public class LocationCodeDropDownRepository : HmcrRepositoryBase<HmrLocationCode>, ILocationCodeDropDownRepository
    {
        public LocationCodeDropDownRepository(AppDbContext dbContext, IMapper mapper)
               : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<LocationCodeDropDownDto>> GetLocationCodes()
        {
            var entity = await DbSet.AsNoTracking()
                .ToListAsync();

            return Mapper.Map<IEnumerable<LocationCodeDropDownDto>>(entity);
        }
    }
}
 