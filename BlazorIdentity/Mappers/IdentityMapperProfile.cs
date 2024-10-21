// Based on the IdentityServer4.EntityFramework - authors - Brock Allen & Dominick Baier.
// https://github.com/IdentityServer/IdentityServer4.EntityFramework

// Modified by Jan Škoruba

using System;
using System.Collections.Generic;
using AutoMapper;
using BlazorIdentity.Users.Models;
using BlazorIdentityApi.Common;
using BlazorIdentityApi.ExceptionHandling;
using Microsoft.AspNetCore.Identity;
using BlazorIdentityApi.Dtos.Identity;


namespace BlazorIdentityApi.Mappers
{
    public class IdentityMapperProfile
        : Profile
    {
        public IdentityMapperProfile()
        {
            // entity to model
            CreateMap<ApplicationUser, UserDto<Guid>>(MemberList.Destination);

            CreateMap<UserLoginInfo, UserProviderDto<Guid>>(MemberList.Destination);

            CreateMap<IdentityError, ViewErrorMessage>(MemberList.Destination)
                .ForMember(x => x.ErrorKey, opt => opt.MapFrom(src => src.Code))
                .ForMember(x => x.ErrorMessage, opt => opt.MapFrom(src => src.Description));

            // entity to model
            CreateMap<ApplicationRole, RoleDto<Guid>>(MemberList.Destination);

            CreateMap<ApplicationUser, ApplicationUser>(MemberList.Destination)
                .ForMember(x => x.SecurityStamp, opt => opt.Ignore());

            CreateMap<ApplicationRole, ApplicationRole>(MemberList.Destination);

            CreateMap<PagedList<ApplicationUser>, UsersDto<UserDto<Guid>, Guid>>(MemberList.Destination)
                .ForMember(x => x.Users,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<ApplicationUserClaim, UserClaimDto<Guid>>(MemberList.Destination)
                .ForMember(x => x.ClaimId, opt => opt.MapFrom(src => src.Id));

            CreateMap<ApplicationUserClaim, UserClaimDto<Guid>>(MemberList.Destination)
                .ForMember(x => x.ClaimId, opt => opt.MapFrom(src => src.Id));

            CreateMap<PagedList<ApplicationRole>, RolesDto<RoleDto<Guid>, Guid>>(MemberList.Destination)
                .ForMember(x => x.Roles,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<ApplicationRole>, RolesDto<RoleDto<Guid>, Guid>>(MemberList.Destination)
                .ForMember(x => x.Roles,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<ApplicationUserClaim>, UserClaimsDto<UserClaimDto<Guid>, Guid>>(MemberList.Destination)
                .ForMember(x => x.Claims,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<ApplicationRoleClaim>, RoleClaimsDto<RoleClaimDto<Guid>, Guid>>(MemberList.Destination)
                .ForMember(x => x.Claims,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<List<UserLoginInfo>, UserProvidersDto<UserProviderDto<Guid>, Guid>>(MemberList.Destination)
                .ForMember(x => x.Providers, opt => opt.MapFrom(src => src));

            CreateMap<UserLoginInfo, UserProviderDto<Guid>>(MemberList.Destination);

            CreateMap<ApplicationRoleClaim, RoleClaimDto<Guid>>(MemberList.Destination)
                .ForMember(x => x.ClaimId, opt => opt.MapFrom(src => src.Id));

            CreateMap<ApplicationRoleClaim, RoleClaimDto<Guid>>(MemberList.Destination)
                .ForMember(x => x.ClaimId, opt => opt.MapFrom(src => src.Id));

            //CreateMap<TUserLogin, TUserProviderDto>(MemberList.Destination);

            // model to entity
            CreateMap<RoleDto<Guid>, ApplicationRole>(MemberList.Source)
                .ForMember(dest => dest.Id, opt => opt.Condition(srs => srs.Id != null)); ;

            CreateMap<RoleClaimDto<Guid>, ApplicationRoleClaim>(MemberList.Source);

            CreateMap<UserClaimDto<Guid>, ApplicationUserClaim>(MemberList.Source)
                .ForMember(x => x.Id,
                    opt => opt.MapFrom(src => src.ClaimId));

            // model to entity
            CreateMap<UserDto<Guid>, ApplicationUser>(MemberList.Source)
                .ForMember(dest => dest.Id, opt => opt.Condition(srs => srs.Id != null)); ;
        }
    }
}