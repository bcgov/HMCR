using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Model.Dtos.ContractServiceArea;
using Hmcr.Model.Dtos.ContractTerm;
using Hmcr.Model.Dtos.District;
using Hmcr.Model.Dtos.MimeType;
using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.Permission;
using Hmcr.Model.Dtos.Region;
using Hmcr.Model.Dtos.Role;
using Hmcr.Model.Dtos.RolePermission;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.ServiceAreaUser;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionStatus;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Dtos.UserRole;

namespace Hmcr.Data.Mappings
{
    public class ModelToEntityProfile : Profile
    {
        public ModelToEntityProfile()
        {
            SourceMemberNamingConvention = new PascalCaseNamingConvention();
            DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();

            SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            DestinationMemberNamingConvention = new PascalCaseNamingConvention();

            CreateMap<ContractServiceAreaDto, HmrContractServiceArea>()
                .ForMember(dest => dest.ServiceAreaNumberNavigation, opt => opt.MapFrom(src => src.ServiceArea));

            CreateMap<ContractTermDto, HmrContractTerm>();

            CreateMap<DistrictDto, HmrDistrict>()
                .ForMember(dest => dest.RegionNumberNavigation, opt => opt.MapFrom(src => src.Region));

            CreateMap<MimeTypeDto, HmrMimeType>();

            CreateMap<PartyDto, HmrParty>();

            CreateMap<PermissionDto, HmrPermission>();

            CreateMap<RegionDto, HmrRegion>();

            CreateMap<RoleDto, HmrRole>();

            CreateMap<RolePermissionDto, HmrRolePermission>();

            CreateMap<ServiceAreaDto, HmrServiceArea>()
                .ForMember(dest => dest.DistrictNumberNavigation, opt => opt.MapFrom(src => src.District));

            CreateMap<ServiceAreaUserDto, HmrServiceAreaUser>()
                .ForMember(dest => dest.ServiceAreaNumberNavigation, opt => opt.MapFrom(src => src.ServiceArea));

            CreateMap<SubmissionObjectDto, HmrSubmissionObject>()
                .ForMember(dest => dest.ServiceAreaNumberNavigation, opt => opt.MapFrom(src => src.ServiceArea));

            CreateMap<SubmissionStatusDto, HmrSubmissionStatu>();

            CreateMap<UserDto, HmrSystemUser>();
            CreateMap<UserCreateDto, HmrSystemUser>();

            CreateMap<UserRoleDto, HmrUserRole>();
        }
    }
}
