using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Model;
using Hmcr.Model.Dtos.ActivityCode;
using Hmcr.Model.Dtos.ActivityRule;
using Hmcr.Model.Dtos.CodeLookup;
using Hmcr.Model.Dtos.ContractTerm;
using Hmcr.Model.Dtos.District;
using Hmcr.Model.Dtos.FeedbackMessage;
using Hmcr.Model.Dtos.LocationCode;
using Hmcr.Model.Dtos.MimeType;
using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.Permission;
using Hmcr.Model.Dtos.Region;
using Hmcr.Model.Dtos.RockfallReport;
using Hmcr.Model.Dtos.Role;
using Hmcr.Model.Dtos.RolePermission;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.ServiceAreaUser;
using Hmcr.Model.Dtos.ServiceAreaActivity;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Dtos.SubmissionStatus;
using Hmcr.Model.Dtos.SubmissionStream;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Dtos.UserRole;
using Hmcr.Model.Dtos.WildlifeReport;
using Hmcr.Model.Dtos.WorkReport;

namespace Hmcr.Data.Mappings
{
    public class EntityToModelProfile : Profile
    {
        public EntityToModelProfile()
        {
            SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            DestinationMemberNamingConvention = new PascalCaseNamingConvention();

            CreateMap<HmrContractTerm, ContractTermDto>();

            CreateMap<HmrDistrict, DistrictDto>();

            CreateMap<HmrMimeType, MimeTypeDto>();

            CreateMap<HmrParty, PartyDto>();

            CreateMap<HmrPermission, PermissionDto>();

            CreateMap<HmrRegion, RegionDto>();

            CreateMap<HmrRole, RoleDto>();
            CreateMap<HmrRole, RoleCreateDto>();
            CreateMap<HmrRole, RoleUpdateDto>();
            CreateMap<HmrRole, RoleSearchDto>();
            CreateMap<HmrRole, RoleDeleteDto>();

            CreateMap<HmrRolePermission, RolePermissionDto>();

            CreateMap<HmrServiceArea, ServiceAreaDto>();
            CreateMap<HmrServiceArea, ServiceAreaNumberDto>();

            CreateMap<HmrServiceAreaUser, ServiceAreaUserDto>();

            CreateMap<HmrSubmissionObject, SubmissionObjectDto>();
            CreateMap<HmrSubmissionObject, SubmissionObjectCreateDto>();
            CreateMap<HmrSubmissionObject, SubmissionObjectFileDto>()
                .ForMember(dst => dst.MimeTypeCode, opt => opt.MapFrom(src => src.MimeType.MimeTypeCode));
            CreateMap<HmrSubmissionObject, SubmissionDto>()
                .ForMember(dst => dst.StagingTableName, opt => opt.MapFrom(src => src.SubmissionStream.StagingTableName))
                .ForMember(dst => dst.MimeTypeCode, opt => opt.MapFrom(src => src.MimeType.MimeTypeCode));

            CreateMap<HmrSubmissionRow, SubmissionRowDto>();

            CreateMap<HmrSubmissionStatu, SubmissionStatusDto>();

            CreateMap<HmrSystemUser, UserDto>();
            CreateMap<HmrSystemUser, UserCreateDto>();
            CreateMap<HmrSystemUser, UserCurrentDto>();
            CreateMap<HmrSystemUser, UserSearchDto>();
            CreateMap<HmrSystemUser, UserUpdateDto>();
            CreateMap<HmrSystemUser, UserDeleteDto>();

            CreateMap<BceidAccount, UserBceidAccountDto>();

            CreateMap<HmrUserRole, UserRoleDto>();

            CreateMap<HmrSubmissionStream, SubmissionStreamDto>();

            CreateMap<HmrActivityCode, ActivityCodeDto>();
            CreateMap<HmrActivityCode, ActivityCodeSearchDto>();
            CreateMap<HmrActivityCode, ActivityCodeCreateDto>();
            CreateMap<HmrActivityCode, ActivityCodeUpdateDto>();

            CreateMap<HmrLocationCode, LocationCodeDto>();
            CreateMap<HmrLocationCode, LocationCodeDropDownDto>();
            
            CreateMap<HmrWorkReport, WorkReportTyped>();
            CreateMap<HmrWorkReport, WorkReportExportDto>();

            CreateMap<HmrRockfallReport, RockfallReportTyped>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.ReporterName))
                .ForMember(dst => dst.McName, opt => opt.MapFrom(src => src.SubmissionObject.Party.BusinessLegalName));

            CreateMap<HmrRockfallReport, RockfallReportExportDto>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.ReporterName))
                .ForMember(dst => dst.McName, opt => opt.MapFrom(src => src.SubmissionObject.Party.BusinessLegalName));

            CreateMap<HmrWildlifeReport, WildlifeReportTyped>();
            CreateMap<HmrWildlifeReport, WildlifeReportExportDto>();

            CreateMap<HmrCodeLookup, CodeLookupDto>();

            CreateMap<HmrFeedbackMessage, FeedbackMessageDto>();
            CreateMap<HmrFeedbackMessage, FeedbackMessageUpdateDto>();

            CreateMap<HmrActivityCodeRule, ActivityCodeRuleDto>();

            CreateMap<HmrServiceAreaActivity, ServiceAreaActivityDto>();
        }
    }
}
