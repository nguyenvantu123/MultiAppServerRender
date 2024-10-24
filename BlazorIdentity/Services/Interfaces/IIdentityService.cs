// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using BlazorIdentityApi.Dtos.Identity;
using WebApp.Models;
using BlazorIdentityApi.Dtos.Identity.Interfaces;

namespace BlazorIdentityApi.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<bool> ExistsUserAsync(string userId);

        Task<bool> ExistsRoleAsync(string roleId);

        Task<UsersDto<UserDto<Guid>, Guid>> GetUsersAsync(string search, int page = 1, int pageSize = 10);
        Task<UserDto<Guid>> GetRoleUsersAsync(string roleId, string search, int page = 1, int pageSize = 10);
        Task<UserDto<Guid>> GetClaimUsersAsync(string claimType, string claimValue, int page = 1, int pageSize = 10);

        Task<RolesDto<RoleDto<Guid>, Guid>> GetRolesAsync(string search, int page = 1, int pageSize = 10);

        Task<(IdentityResult identityResult, Guid roleId)> CreateRoleAsync(RoleDto<Guid> role);

        Task<RoleDto<Guid>> GetRoleAsync(string roleId);

        Task<List<RoleDto<Guid>>> GetRolesAsync();

        Task<(IdentityResult identityResult, Guid roleId)> UpdateRoleAsync(RoleDto<Guid> role);

        Task<UserDto<Guid>> GetUserAsync(string userId);

        Task<(IdentityResult identityResult, Guid userId)> CreateUserAsync(UserDto<Guid> user);

        Task<(IdentityResult identityResult, Guid userId)> UpdateUserAsync(UserDto<Guid> user);

        Task<IdentityResult> DeleteUserAsync(string userId, UserDto<Guid> user);

        Task<IdentityResult> CreateUserRoleAsync(UserRolesDto<RoleDto<Guid>, Guid> role);

        Task<UserRolesDto<RoleDto<Guid>, Guid>> BuildUserRolesViewModel(Guid id, int? page);

        Task<UserRolesDto<RoleDto<Guid>, Guid>> GetUserRolesAsync(string userId, int page = 1,
            int pageSize = 10);

        Task<IdentityResult> DeleteUserRoleAsync(UserRolesDto<RoleDto<Guid>, Guid> role);

        Task<UserClaimsDto<UserClaimDto<Guid>, Guid>> GetUserClaimsAsync(string userId, int page = 1,
            int pageSize = 10);

        Task<UserClaimsDto<UserClaimDto<Guid>, Guid>> GetUserClaimAsync(string userId, int claimId);

        Task<IdentityResult> CreateUserClaimsAsync(UserClaimDto<Guid> claimsDto);

        Task<IdentityResult> UpdateUserClaimsAsync(UserClaimDto<Guid> claimsDto);

        Task<IdentityResult> DeleteUserClaimAsync(UserClaimDto<Guid> claim);

        Task<UserProviderDto<Guid>> GetUserProvidersAsync(string userId);

        Guid ConvertToKeyFromString(string id);

        Task<IdentityResult> DeleteUserProvidersAsync(UserProviderDto<Guid> provider);

        //Task<UserProviderDto> GetUserProviderAsync(string userId, string providerKey);

        Task<IdentityResult> UserChangePasswordAsync(UserChangePasswordDto<Guid> userPassword);

        Task<IdentityResult> CreateRoleClaimsAsync(RoleClaimDto<Guid> claimsDto);

        Task<IdentityResult> UpdateRoleClaimsAsync(RoleClaimDto<Guid> claimsDto);

        Task<RoleClaimsDto<RoleClaimDto<Guid>, Guid>> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10);

        Task<RoleClaimsDto<RoleClaimDto<Guid>, Guid>> GetUserRoleClaimsAsync(string userId, string claimSearchText, int page = 1, int pageSize = 10);

        Task<RoleClaimsDto<RoleClaimDto<Guid>, Guid>> GetRoleClaimAsync(string roleId, int claimId);

        Task<IdentityResult> DeleteRoleClaimAsync(RoleClaimDto<Guid> role);

        Task<IdentityResult> DeleteRoleAsync(RoleDto<Guid> role);
    }
}