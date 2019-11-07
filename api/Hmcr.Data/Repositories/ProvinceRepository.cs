using AutoMapper;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IProvinceRepository : IHmcrRepositoryBase<ProvinceDto, Province>
    {
        PagedDto<ProvinceDto> GetProvincesByPage(int pageSize, int pageNumber);
    }

    public class ProvinceRepository : HmcrRepositoryBase<ProvinceDto, Province>, IProvinceRepository
    {
        public ProvinceRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public PagedDto<ProvinceDto> GetProvincesByPage(int pageSize, int pageNumber)
        {
            return Page<Province, ProvinceDto>(DbSet.AsQueryable(), pageSize, pageNumber, "ProvinceId DESC");
        }
    }
}