using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Model.Dtos.LocationCode;
using Hmcr.Model.Dtos.CodeLookup;
using Hmcr.Model.Dtos.ContractTerm;
using Hmcr.Model.Dtos.District;
using Hmcr.Model.Dtos.FeedbackMessage;
using Hmcr.Model.Dtos.MimeType;
using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.Permission;
using Hmcr.Model.Dtos.Region;
using Hmcr.Model.Dtos.RockfallReport;
using Hmcr.Model.Dtos.Role;
using Hmcr.Model.Dtos.RolePermission;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.ServiceAreaUser;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Dtos.SubmissionStatus;
using Hmcr.Model.Dtos.SubmissionStream;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Dtos.UserRole;
using Hmcr.Model.Dtos.WildlifeReport;
using Hmcr.Model.Dtos.WorkReport;
using Hmcr.Model.Dtos.ActivityCode;

namespace Hmcr.Data.Mappings
{
    public class ModelToEntityProfile : Profile
    {
        public ModelToEntityProfile()
        {
            //SourceMemberNamingConvention = new PascalCaseNamingConvention();
            //DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();

            SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            DestinationMemberNamingConvention = new PascalCaseNamingConvention();

            CreateMap<ContractTermDto, HmrContractTerm>();

            CreateMap<DistrictDto, HmrDistrict>();

            CreateMap<MimeTypeDto, HmrMimeType>();

            CreateMap<PartyDto, HmrParty>();

            CreateMap<PermissionDto, HmrPermission>();

            CreateMap<RegionDto, HmrRegion>();

            CreateMap<RoleDto, HmrRole>();
            CreateMap<RoleCreateDto, HmrRole>();
            CreateMap<RoleUpdateDto, HmrRole>();
            CreateMap<RoleSearchDto, HmrRole>();
            CreateMap<RoleDeleteDto, HmrRole>();

            CreateMap<RolePermissionDto, HmrRolePermission>();

            CreateMap<ServiceAreaDto, HmrServiceArea>();
            CreateMap<ServiceAreaNumberDto, HmrServiceArea>();

            CreateMap<ServiceAreaUserDto, HmrServiceAreaUser>();

            CreateMap<SubmissionObjectDto, HmrSubmissionObject>();
            CreateMap<SubmissionObjectCreateDto, HmrSubmissionObject>();

            CreateMap<SubmissionRowDto, HmrSubmissionRow>();

            CreateMap<SubmissionStatusDto, HmrSubmissionStatu>();

            CreateMap<UserDto, HmrSystemUser>();
            CreateMap<UserCreateDto, HmrSystemUser>();
            CreateMap<UserCurrentDto, HmrSystemUser>();
            CreateMap<UserSearchDto, HmrSystemUser>();
            CreateMap<UserUpdateDto, HmrSystemUser>();
            CreateMap<UserDeleteDto, HmrSystemUser>();

            CreateMap<UserRoleDto, HmrUserRole>();

            CreateMap<SubmissionStreamDto, HmrSubmissionStream>();

            CreateMap<ActivityCodeDto, HmrActivityCode>();
            CreateMap<ActivityCodeSearchDto, HmrActivityCode>();
            CreateMap<ActivityCodeCreateDto, HmrActivityCode>();
            CreateMap<ActivityCodeUpdateDto, HmrActivityCode>();
            CreateMap<LocationCodeDto, HmrLocationCode>();

            CreateMap<WorkReportTyped, HmrWorkReport>();

            CreateMap<RockfallReportTyped, HmrRockfallReport>()
                .ForMember(dst => dst.ReporterName, opt => opt.MapFrom(src => src.Name));

            CreateMap<WildlifeReportTyped, HmrWildlifeReport>();

            CreateMap<CodeLookupDto, HmrCodeLookup>();

            CreateMap<FeedbackMessageDto, HmrFeedbackMessage>();
            CreateMap<FeedbackMessageUpdateDto, HmrFeedbackMessage>();
        }
    }
}
