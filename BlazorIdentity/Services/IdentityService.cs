// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorIdentity.Repositories.Interfaces;
using BlazorIdentity.Users.Models;
using BlazorIdentityApi.Common;
using BlazorIdentityApi.Dtos.Identity;
using BlazorIdentityApi.ExceptionHandling;
using BlazorIdentityApi.Resources;
using BlazorIdentityApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using WebApp.Models;


namespace BlazorIdentityApi.Services
{
    public class IdentityService : IIdentityService
    {
        protected readonly IIdentityServiceResources IdentityServiceResources;
        protected readonly IMapper Mapper;
        protected readonly IIdentityRepository IdentityRepository;

        public IdentityService(IIdentityRepository identityRepository,
            IIdentityServiceResources identityServiceResources,
            IMapper mapper)
        {
            IdentityRepository = identityRepository;
            IdentityServiceResources = identityServiceResources;
            Mapper = mapper;
        }

        public virtual async Task<bool> ExistsUserAsync(string userId)
        {
            var exists = await IdentityRepository.ExistsUserAsync(userId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            return true;
        }

        public virtual async Task<bool> ExistsRoleAsync(string roleId)
        {
            var exists = await IdentityRepository.ExistsRoleAsync(roleId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleDoesNotExist().Description, roleId), IdentityServiceResources.RoleDoesNotExist().Description);

            return true;
        }

        public virtual async Task<UsersDto<UserDto<Guid>, Guid>> GetUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await IdentityRepository.GetUsersAsync(search, page, pageSize);
            var usersDto = Mapper.Map<UsersDto<UserDto<Guid>, Guid>>(pagedList);

            //await AuditEventLogger.LogEventAsync(new UsersRequestedEvent<UserDto<Guid>>(usersDto));

            return usersDto;
        }

        public virtual async Task<UsersDto<UserDto<Guid>, Guid>> GetRoleUsersAsync(string roleId, string search, int page = 1, int pageSize = 10)
        {
            var roleKey = ConvertToKeyFromString(roleId);

            var userIdentityRole = await IdentityRepository.GetRoleAsync(roleKey);
            if (userIdentityRole == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleDoesNotExist().Description, roleId), IdentityServiceResources.RoleDoesNotExist().Description);

            var pagedList = await IdentityRepository.GetRoleUsersAsync(roleId, search, page, pageSize);
            var usersDto = Mapper.Map<UsersDto<UserDto<Guid>, Guid>>(pagedList);

            //

            return usersDto;
        }

        public virtual async Task<UserDto<Guid>> GetClaimUsersAsync(string claimType, string claimValue, int page = 1, int pageSize = 10)
        {
            var pagedList = await IdentityRepository.GetClaimUsersAsync(claimType, claimValue, page, pageSize);
            var usersDto = Mapper.Map<UserDto<Guid>>(pagedList);

            //await AuditEventLogger.LogEventAsync(new ClaimUsersRequestedEvent<UserDto<Guid>>(usersDto));

            return usersDto;
        }

        public virtual async Task<RolesDto<RoleDto<Guid>, Guid>> GetRolesAsync(string search, int page = 1, int pageSize = 10)
        {
            PagedList<ApplicationRole> pagedList = await IdentityRepository.GetRolesAsync(search, page, pageSize);
            var rolesDto = Mapper.Map<RolesDto<RoleDto<Guid>, Guid>>(pagedList);

            //await AuditEventLogger.LogEventAsync(new RolesRequestedEvent<RoleDto<Guid>>(rolesDto));

            return rolesDto;
        }

        public virtual async Task<(IdentityResult identityResult, Guid roleId)> CreateRoleAsync(RoleDto<Guid> role)
        {
            var roleEntity = Mapper.Map<ApplicationRole>(role);
            var (identityResult, roleId) = await IdentityRepository.CreateRoleAsync(roleEntity);
            var handleIdentityError = HandleIdentityError(identityResult, IdentityServiceResources.RoleCreateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, role);


            return (handleIdentityError, roleId);
        }

        private IdentityResult HandleIdentityError(IdentityResult identityResult, string errorMessage, string errorKey, object model)
        {
            if (!identityResult.Errors.Any()) return identityResult;
            var viewErrorMessages = Mapper.Map<List<ViewErrorMessage>>(identityResult.Errors);

            throw new UserFriendlyViewException(errorMessage, errorKey, viewErrorMessages, model);
        }

        public virtual async Task<RoleDto<Guid>> GetRoleAsync(string roleId)
        {
            var roleKey = ConvertToKeyFromString(roleId);

            var userIdentityRole = await IdentityRepository.GetRoleAsync(roleKey);
            if (userIdentityRole == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleDoesNotExist().Description, roleId), IdentityServiceResources.RoleDoesNotExist().Description);

            var roleDto = Mapper.Map<RoleDto<Guid>>(userIdentityRole);


            return roleDto;
        }

        public virtual async Task<List<RoleDto<Guid>>> GetRolesAsync()
        {
            var roles = await IdentityRepository.GetRolesAsync();
            var roleDtos = Mapper.Map<List<RoleDto<Guid>>>(roles);


            return roleDtos;
        }

        public virtual async Task<(IdentityResult identityResult, Guid roleId)> UpdateRoleAsync(RoleDto<Guid> role)
        {
            var userIdentityRole = Mapper.Map<ApplicationRole>(role);

            var originalRole = await GetRoleAsync(role.Id.ToString());

            var (identityResult, roleId) = await IdentityRepository.UpdateRoleAsync(userIdentityRole);

            //await AuditEventLogger.LogEventAsync(new RoleUpdatedEvent<TRoleDto>(originalRole, role));

            var handleIdentityError = HandleIdentityError(identityResult, IdentityServiceResources.RoleUpdateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, role);

            return (handleIdentityError, roleId);
        }

        public virtual async Task<UserDto<Guid>> GetUserAsync(string userId)
        {
            var identity = await IdentityRepository.GetUserAsync(userId);
            if (identity == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var userDto = Mapper.Map<UserDto<Guid>>(identity);

            return userDto;
        }

        public virtual async Task<(IdentityResult identityResult, Guid userId)> CreateUserAsync(UserDto<Guid> user)
        {
            var userIdentity = Mapper.Map<ApplicationUser>(user);
            var (identityResult, userId) = await IdentityRepository.CreateUserAsync(userIdentity);

            var handleIdentityError = HandleIdentityError(identityResult, IdentityServiceResources.UserCreateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, user);

            //await AuditEventLogger.LogEventAsync(new UserSavedEvent<TUserDto>(user));

            return (handleIdentityError, userId);
        }

        /// <summary>
        /// Updates the specified user, but without updating the password hash value
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<(IdentityResult identityResult, Guid userId)> UpdateUserAsync(UserDto<Guid> user)
        {
            var userIdentity = Mapper.Map<ApplicationUser>(user);
            await MapOriginalPasswordHashAsync(userIdentity);

            var originalUser = await GetUserAsync(user.Id.ToString());

            var (identityResult, userId) = await IdentityRepository.UpdateUserAsync(userIdentity);
            var handleIdentityError = HandleIdentityError(identityResult, IdentityServiceResources.UserUpdateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, user);

            //await AuditEventLogger.LogEventAsync(new UserUpdatedEvent<TUserDto>(originalUser, user));

            return (handleIdentityError, userId);
        }

        /// <summary>
        /// Get original password hash and map password hash to user
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        private async Task MapOriginalPasswordHashAsync(ApplicationUser userIdentity)
        {
            var identity = await IdentityRepository.GetUserAsync(userIdentity.Id.ToString());
            userIdentity.PasswordHash = identity.PasswordHash;
        }

        public virtual async Task<IdentityResult> DeleteUserAsync(string userId, UserDto<Guid> user)
        {
            var identityResult = await IdentityRepository.DeleteUserAsync(userId);

            //await AuditEventLogger.LogEventAsync(new UserDeletedEvent<TUserDto>(user));

            return HandleIdentityError(identityResult, IdentityServiceResources.UserDeleteFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, user);
        }

        public virtual async Task<IdentityResult> CreateUserRoleAsync(UserRolesDto<RoleDto<Guid>, Guid> role)
        {
            var identityResult = await IdentityRepository.CreateUserRoleAsync(role.UserId.ToString(), role.RoleId.ToString());

            //await AuditEventLogger.LogEventAsync(new UserRoleSavedEvent<TUserRolesDto>(role));

            if (!identityResult.Errors.Any()) return identityResult;

            var userRolesDto = await BuildUserRolesViewModel(role.UserId, 1);

            return HandleIdentityError(identityResult, IdentityServiceResources.UserRoleCreateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, userRolesDto);
        }

        public virtual async Task<UserRolesDto<RoleDto<Guid>, Guid>> BuildUserRolesViewModel(Guid id, int? page)
        {
            var roles = await GetRolesAsync();
            var userRoles = await GetUserRolesAsync(id.ToString(), page ?? 1);
            userRoles.UserId = id;
            userRoles.RolesList = roles.Select(x => new SelectItemDto(x.Id.ToString(), x.Name)).ToList();

            return userRoles;
        }

        public virtual async Task<UserRolesDto<RoleDto<Guid>, Guid>> GetUserRolesAsync(string userId, int page = 1, int pageSize = 10)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var userIdentityRoles = await IdentityRepository.GetUserRolesAsync(userId, page, pageSize);
            var roleDtos = Mapper.Map<UserRolesDto<RoleDto<Guid>, Guid>>(userIdentityRoles);

            var user = await IdentityRepository.GetUserAsync(userId);
            roleDtos.UserName = user.UserName;

            //await AuditEventLogger.LogEventAsync(new UserRolesRequestedEvent<TUserRolesDto>(roleDtos));

            return roleDtos;
        }

        public virtual async Task<IdentityResult> DeleteUserRoleAsync(UserRolesDto<RoleDto<Guid>, Guid> role)
        {
            var identityResult = await IdentityRepository.DeleteUserRoleAsync(role.UserId.ToString(), role.RoleId.ToString());

            //await AuditEventLogger.LogEventAsync(new UserRoleDeletedEvent<TUserRolesDto>(role));

            return HandleIdentityError(identityResult, IdentityServiceResources.UserRoleDeleteFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, role);
        }

        public virtual async Task<UserClaimsDto<UserClaimDto<Guid>, Guid>> GetUserClaimsAsync(string userId, int page = 1, int pageSize = 10)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var identityUserClaims = await IdentityRepository.GetUserClaimsAsync(userId, page, pageSize);
            var claimDtos = Mapper.Map<UserClaimsDto<UserClaimDto<Guid>, Guid>>(identityUserClaims);

            var user = await IdentityRepository.GetUserAsync(userId);
            claimDtos.UserName = user.UserName;

            //await AuditEventLogger.LogEventAsync(new UserClaimsRequestedEvent<TUserClaimsDto>(claimDtos));

            return claimDtos;
        }

        public virtual async Task<UserClaimsDto<UserClaimDto<Guid>, Guid>> GetUserClaimAsync(string userId, int claimId)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var identityUserClaim = await IdentityRepository.GetUserClaimAsync(userId, claimId);
            if (identityUserClaim == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserClaimDoesNotExist().Description, userId), IdentityServiceResources.UserClaimDoesNotExist().Description);

            var userClaimsDto = Mapper.Map<UserClaimsDto<UserClaimDto<Guid>, Guid>>(identityUserClaim);

            //await AuditEventLogger.LogEventAsync(new UserClaimRequestedEvent<TUserClaimsDto>(userClaimsDto));

            return userClaimsDto;
        }

        public virtual async Task<IdentityResult> CreateUserClaimsAsync(UserClaimDto<Guid> claimsDto)
        {
            var userIdentityUserClaim = Mapper.Map<ApplicationUserClaim>(claimsDto);
            var identityResult = await IdentityRepository.CreateUserClaimsAsync(userIdentityUserClaim);

            //await AuditEventLogger.LogEventAsync(new UserClaimsSavedEvent<TUserClaimsDto>(claimsDto));

            return HandleIdentityError(identityResult, IdentityServiceResources.UserClaimsCreateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, claimsDto);
        }

        public virtual async Task<IdentityResult> UpdateUserClaimsAsync(UserClaimDto<Guid> claimsDto)
        {
            var userIdentityUserClaim = Mapper.Map<ApplicationUserClaim>(claimsDto);
            var identityResult = await IdentityRepository.UpdateUserClaimsAsync(userIdentityUserClaim);

            //await AuditEventLogger.LogEventAsync(new UserClaimsSavedEvent<TUserClaimsDto>(claimsDto));

            return HandleIdentityError(identityResult, IdentityServiceResources.UserClaimsUpdateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, claimsDto);
        }

        public virtual async Task<IdentityResult> DeleteUserClaimAsync(UserClaimDto<Guid> claim)
        {
            var deleted = await IdentityRepository.DeleteUserClaimAsync(claim.UserId.ToString(), claim.ClaimId);

            //await AuditEventLogger.LogEventAsync(new UserClaimsDeletedEvent<TUserClaimsDto>(claim));

            return deleted;
        }

        public virtual Guid ConvertToKeyFromString(string id)
        {
            if (id == null)
            {
                return Guid.Empty;
            }
            return (Guid)TypeDescriptor.GetConverter(typeof(Guid))!.ConvertFromInvariantString(id);
        }

        public virtual async Task<UserProvidersDto<UserProviderDto<Guid>, Guid>> GetUserProvidersAsync(string userId)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var userLoginInfos = await IdentityRepository.GetUserProvidersAsync(userId);
            var providersDto = Mapper.Map<UserProvidersDto<UserProviderDto<Guid>, Guid>>(userLoginInfos);
            providersDto.UserId = ConvertToKeyFromString(userId);

            var user = await IdentityRepository.GetUserAsync(userId);
            providersDto.UserName = user.UserName;

            //await AuditEventLogger.LogEventAsync(new UserProvidersRequestedEvent<TUserProvidersDto>(providersDto));

            return providersDto;
        }

        public virtual async Task<IdentityResult> DeleteUserProvidersAsync(UserProviderDto<Guid> provider)
        {
            var identityResult = await IdentityRepository.DeleteUserProvidersAsync(provider.UserId.ToString(), provider.ProviderKey, provider.LoginProvider);

            //await AuditEventLogger.LogEventAsync(new UserProvidersDeletedEvent<TUserProviderDto>(provider));

            return HandleIdentityError(identityResult, IdentityServiceResources.UserProviderDeleteFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, provider);
        }

        //public virtual async Task<TUserProviderDto> GetUserProviderAsync(string userId, string providerKey)
        //{
        //    var userExists = await IdentityRepository.ExistsUserAsync(userId);
        //    if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

        //    var identityUserLogin = await IdentityRepository.GetUserProviderAsync(userId, providerKey);
        //    if (identityUserLogin == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserProviderDoesNotExist().Description, providerKey), IdentityServiceResources.UserProviderDoesNotExist().Description);

        //    var userProviderDto = Mapper.Map<TUserProviderDto>(identityUserLogin);
        //    var user = await GetUserAsync(userId);
        //    userProviderDto.UserName = user.UserName;

        //    //await AuditEventLogger.LogEventAsync(new UserProviderRequestedEvent<TUserProviderDto>(userProviderDto));

        //    return userProviderDto;
        //}

        public virtual async Task<IdentityResult> UserChangePasswordAsync(UserChangePasswordDto<Guid> userPassword)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userPassword.UserId.ToString());
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userPassword.UserId), IdentityServiceResources.UserDoesNotExist().Description);

            var identityResult = await IdentityRepository.UserChangePasswordAsync(userPassword.UserId.ToString(), userPassword.Password);

            //await AuditEventLogger.LogEventAsync(new UserPasswordChangedEvent(userPassword.UserName));

            return HandleIdentityError(identityResult, IdentityServiceResources.UserChangePasswordFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, userPassword);
        }

        public virtual async Task<IdentityResult> CreateRoleClaimsAsync(RoleClaimDto<Guid> claimsDto)
        {
            var identityRoleClaim = Mapper.Map<ApplicationRoleClaim>(claimsDto);
            var identityResult = await IdentityRepository.CreateRoleClaimsAsync(identityRoleClaim);

            //await AuditEventLogger.LogEventAsync(new RoleClaimsSavedEvent<TRoleClaimsDto>(claimsDto));

            return HandleIdentityError(identityResult, IdentityServiceResources.RoleClaimsCreateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, claimsDto);
        }

        public virtual async Task<IdentityResult> UpdateRoleClaimsAsync(RoleClaimDto<Guid> claimsDto)
        {
            var identityRoleClaim = Mapper.Map<ApplicationRoleClaim>(claimsDto);
            var identityResult = await IdentityRepository.UpdateRoleClaimsAsync(identityRoleClaim);

            //await AuditEventLogger.LogEventAsync(new RoleClaimsSavedEvent<TRoleClaimsDto>(claimsDto));

            return HandleIdentityError(identityResult, IdentityServiceResources.RoleClaimsUpdateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, claimsDto);
        }

        public virtual async Task<RoleClaimsDto<RoleClaimDto<Guid>, Guid>> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10)
        {
            var roleExists = await IdentityRepository.ExistsRoleAsync(roleId);
            if (!roleExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleDoesNotExist().Description, roleId), IdentityServiceResources.RoleDoesNotExist().Description);

            var identityRoleClaims = await IdentityRepository.GetRoleClaimsAsync(roleId, page, pageSize);
            var roleClaimDtos = Mapper.Map<RoleClaimsDto<RoleClaimDto<Guid>, Guid>>(identityRoleClaims);
            var roleDto = await GetRoleAsync(roleId);
            roleClaimDtos.RoleId = roleDto.Id;

            return roleClaimDtos;
        }

        public virtual async Task<RoleClaimsDto<RoleClaimDto<Guid>, Guid>> GetUserRoleClaimsAsync(string userId, string claimSearchText, int page = 1, int pageSize = 10)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var identityRoleClaims = await IdentityRepository.GetUserRoleClaimsAsync(userId, claimSearchText, page, pageSize);
            var roleClaimDtos = Mapper.Map<RoleClaimsDto<RoleClaimDto<Guid>, Guid>>(identityRoleClaims);

            return roleClaimDtos;
        }

        public virtual async Task<RoleClaimsDto<RoleClaimDto<Guid>, Guid>> GetRoleClaimAsync(string roleId, int claimId)
        {
            var roleExists = await IdentityRepository.ExistsRoleAsync(roleId);
            if (!roleExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleDoesNotExist().Description, roleId), IdentityServiceResources.RoleDoesNotExist().Description);

            var identityRoleClaim = await IdentityRepository.GetRoleClaimAsync(roleId, claimId);
            if (identityRoleClaim == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleClaimDoesNotExist().Description, claimId), IdentityServiceResources.RoleClaimDoesNotExist().Description);
            var roleClaimsDto = Mapper.Map<RoleClaimsDto<RoleClaimDto<Guid>, Guid>>(identityRoleClaim);
            var roleDto = await GetRoleAsync(roleId);
            roleClaimsDto.RoleName = roleDto.Name;

            //await AuditEventLogger.LogEventAsync(new RoleClaimRequestedEvent<TRoleClaimsDto>(roleClaimsDto));

            return roleClaimsDto;
        }

        public virtual async Task<IdentityResult> DeleteRoleClaimAsync(RoleClaimDto<Guid> role)
        {
            var deleted = await IdentityRepository.DeleteRoleClaimAsync(role.RoleId.ToString(), role.ClaimId);

            //await AuditEventLogger.LogEventAsync(new RoleClaimsDeletedEvent<TRoleClaimsDto>(role));

            return deleted;
        }

        public virtual async Task<IdentityResult> DeleteRoleAsync(RoleDto<Guid> role)
        {
            var userIdentityRole = Mapper.Map<ApplicationRole>(role);
            var identityResult = await IdentityRepository.DeleteRoleAsync(userIdentityRole);

            //await AuditEventLogger.LogEventAsync(new RoleDeletedEvent<TRoleDto>(role));

            return HandleIdentityError(identityResult, IdentityServiceResources.RoleDeleteFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, role);
        }

        //Task<UserClaimDto<Guid>> IIdentityService.GetUserClaimAsync(string userId, int claimId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}