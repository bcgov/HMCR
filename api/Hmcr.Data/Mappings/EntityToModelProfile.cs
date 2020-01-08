﻿using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Model.Dtos.ActivityCode;
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
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Dtos.SubmissionStatus;
using Hmcr.Model.Dtos.SubmissionStream;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Dtos.UserRole;
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

            CreateMap<HmrSubmissionRow, SubmissionRowDto>();

            CreateMap<HmrSubmissionStatu, SubmissionStatusDto>();

            CreateMap<HmrSystemUser, UserDto>();
            CreateMap<HmrSystemUser, UserCreateDto>();
            CreateMap<HmrSystemUser, UserCurrentDto>();
            CreateMap<HmrSystemUser, UserSearchDto>();
            CreateMap<HmrSystemUser, UserUpdateDto>();
            CreateMap<HmrSystemUser, UserDeleteDto>();

            CreateMap<HmrUserRole, UserRoleDto>();

            CreateMap<HmrSubmissionStream, SubmissionStreamDto>();

            CreateMap<HmrActivityCode, ActivityCodeDto>();
            CreateMap<HmrLocationCode, LocationCodeDto>();

            CreateMap<HmrWorkReport, WorkReportDto>();
        }
    }
}
