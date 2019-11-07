using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Model.Dtos;

namespace Hmcr.Data.Mappings
{
    public class ModelToEntityProfile : Profile
    {
        public ModelToEntityProfile()
        {
            SourceMemberNamingConvention = new PascalCaseNamingConvention();
            DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();

            CreateMap<CountryDto, Country>();
            CreateMap<ProvinceDto, Province>();
        }
    }
}
