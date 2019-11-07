using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Model.Dtos;

namespace Hmcr.Data.Mappings
{
    public class EntityToModelProfile : Profile
    {
        public EntityToModelProfile()
        {
            SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            DestinationMemberNamingConvention = new PascalCaseNamingConvention();

            CreateMap<Country, CountryDto>();
            CreateMap<Province, ProvinceDto>();
        }
    }
}
