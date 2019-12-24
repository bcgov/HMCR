using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.ServiceArea;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IServiceAreaRepository
    {
        Task<IEnumerable<ServiceAreaDto>> GetServiceAreaBySystemUserIdAsync(long systemUserId);
        Task<IEnumerable<ServiceAreaNumberDto>> GetAllServiceAreasAsync();
        IEnumerable<ServiceAreaNumberDto> GetAllServiceAreas();
        Task<int> CountServiceAreaNumbersAsync(IEnumerable<decimal> serviceAreaNumbers);
    }

    public class ServiceAreaRepository : HmcrRepositoryBase<HmrServiceArea>, IServiceAreaRepository
    {
        public ServiceAreaRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<ServiceAreaDto>> GetServiceAreaBySystemUserIdAsync(long systemUserId)
        {
            var entity = await DbSet.AsNoTracking()
                .Where(s => s.HmrServiceAreaUsers.Any(sa => sa.SystemUserId == systemUserId))
                .ToListAsync();

            return Mapper.Map<IEnumerable<ServiceAreaDto>>(entity);
        }

        public async Task<IEnumerable<ServiceAreaNumberDto>> GetAllServiceAreasAsync()
        {
            return await GetAllAsync<ServiceAreaNumberDto>();
        }

        public IEnumerable<ServiceAreaNumberDto> GetAllServiceAreas()
        {
            return GetAll<ServiceAreaNumberDto>();
        }

        public async Task<int> CountServiceAreaNumbersAsync(IEnumerable<decimal> serviceAreaNumbers)
        {
            return await DbSet.CountAsync(s => serviceAreaNumbers.Contains(s.ServiceAreaNumber));
        }
    }
}
