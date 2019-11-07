using AutoMapper;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface ICountryRepository : IHmcrRepositoryBase<CountryDto, Country>
    {
        Task<CountryDto> GetCountryByIdAsync(int countryId);
        CountryDto GetCountryById(int countryId);
    }

    public class CountryRepository : HmcrRepositoryBase<CountryDto, Country>, ICountryRepository
    {
        public CountryRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {   
        }

        public async Task<CountryDto> GetCountryByIdAsync(int countryId)
        {
            return Mapper.Map<CountryDto>(
                await DbSet.Include(c => c.Provinces).FirstOrDefaultAsync(c => c.CountryId == countryId)
            );
        }

        public CountryDto GetCountryById(int countryId)
        {
            return Mapper.Map<CountryDto>(
                DbSet.Include(c => c.Provinces).FirstOrDefault(c => c.CountryId == countryId)
            );
        }
    }
}
