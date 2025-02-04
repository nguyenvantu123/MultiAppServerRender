// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BlazorIdentity.Data;
using BlazorIdentity.Repositories.Interfaces;
using BlazorIdentity.Users.Models;
using BlazorIdentityApi.Common;
using BlazorIdentityApi.Dtos.Enums;
using BlazorIdentityApi.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace BlazorIdentity.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        protected readonly ApplicationDbContext DbContext;
        protected readonly UserManager<ApplicationUser> UserManager;
        protected readonly RoleManager<ApplicationRole> RoleManager;
        //protected readonly IMapper Mapper;

        public bool AutoSaveChanges { get; set; } = true;

        public IdentityRepository(ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager
            //IMapper mapper
            )
        {
            DbContext = dbContext;
            UserManager = userManager;
            RoleManager = roleManager;
            //Mapper = mapper;
        }

        public virtual Guid ConvertKeyFromString(string id)
        {
            if (id == null)
            {
                return default;
            }
            return (Guid)TypeDescriptor.GetConverter(typeof(Guid)).ConvertFromInvariantString(id);
        }

        public virtual Task<bool> ExistsUserAsync(string userId)
        {
            var id = ConvertKeyFromString(userId);

            return UserManager.Users.AnyAsync(x => x.Id.Equals(id));
        }

        public virtual Task<bool> ExistsRoleAsync(string roleId)
        {
            var id = ConvertKeyFromString(roleId);

            return RoleManager.Roles.AnyAsync(x => x.Id.Equals(id));
        }

        public virtual async Task<PagedList<ApplicationUser>> GetUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApplicationUser>();
            Expression<Func<ApplicationUser, bool>> searchCondition = x => x.UserName.Contains(search) || x.Email.Contains(search);

            var users = await UserManager.Users.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Id, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(users);

            pagedList.TotalCount = await UserManager.Users.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<PagedList<ApplicationUser>> GetRoleUsersAsync(string roleId, string search, int page = 1, int pageSize = 10)
        {
            var id = ConvertKeyFromString(roleId);

            var pagedList = new PagedList<ApplicationUser>();
            var users = DbContext.Set<ApplicationUser>()
                .Join(DbContext.Set<ApplicationUserRole>(), u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                .Where(t => t.ur.RoleId.Equals(id))
                .WhereIf(!string.IsNullOrEmpty(search), t => t.u.UserName.Contains(search) || t.u.Email.Contains(search))
                .Select(t => t.u);

            var pagedUsers = await users.PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(pagedUsers);
            pagedList.TotalCount = await users.CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<PagedList<ApplicationUser>> GetClaimUsersAsync(string claimType, string claimValue, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApplicationUser>();
            var users = DbContext.Set<ApplicationUser>()
                .Join(DbContext.Set<ApplicationUserClaim>(), u => u.Id, uc => uc.UserId, (u, uc) => new { u, uc })
                .Where(t => t.uc.ClaimType.Equals(claimType))
                .WhereIf(!string.IsNullOrEmpty(claimValue), t => t.uc.ClaimValue.Equals(claimValue))
                .Select(t => t.u).Distinct();

            var pagedUsers = await users.PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(pagedUsers);
            pagedList.TotalCount = await users.CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<List<ApplicationRole>> GetRolesAsync()
        {
            return RoleManager.Roles.ToListAsync();
        }

        public virtual async Task<PagedList<ApplicationRole>> GetRolesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApplicationRole>();

            Expression<Func<ApplicationRole, bool>> searchCondition = x => x.Name.Contains(search);
            var roles = await RoleManager.Roles.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Id, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(roles);
            pagedList.TotalCount = await RoleManager.Roles.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<ApplicationRole> GetRoleAsync(Guid roleId)
        {
            return RoleManager.Roles.Where(x => x.Id.Equals(roleId)).SingleOrDefaultAsync();
        }

        public virtual async Task<(IdentityResult identityResult, Guid roleId)> CreateRoleAsync(ApplicationRole role)
        {
            var identityResult = await RoleManager.CreateAsync(role);

            return (identityResult, role.Id);
        }

        public virtual async Task<(IdentityResult identityResult, Guid roleId)> UpdateRoleAsync(ApplicationRole role)
        {
            var existingRole = await RoleManager.FindByIdAsync(role.Id.ToString());
            //Mapper.Map(role, existingRole);
            var identityResult = await RoleManager.UpdateAsync(existingRole);

            return (identityResult, role.Id);
        }

        public virtual async Task<IdentityResult> DeleteRoleAsync(ApplicationRole role)
        {
            var thisRole = await RoleManager.FindByIdAsync(role.Id.ToString());

            return await RoleManager.DeleteAsync(thisRole);
        }

        public virtual Task<ApplicationUser> GetUserAsync(string userId)
        {
            return UserManager.FindByIdAsync(userId);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>This method returns identity result and new user id</returns>
        public virtual async Task<(IdentityResult identityResult, Guid userId)> CreateUserAsync(ApplicationUser user)
        {
            var identityResult = await UserManager.CreateAsync(user);

            return (identityResult, user.Id);
        }

        public virtual async Task<(IdentityResult identityResult, Guid userId)> UpdateUserAsync(ApplicationUser user)
        {
            var userIdentity = await UserManager.FindByIdAsync(user.Id.ToString());
            //Mapper.Map(user, userIdentity);
            var identityResult = await UserManager.UpdateAsync(userIdentity);

            return (identityResult, user.Id);
        }

        public virtual async Task<IdentityResult> CreateUserRoleAsync(string userId, string roleId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var selectRole = await RoleManager.FindByIdAsync(roleId);

            return await UserManager.AddToRoleAsync(user, selectRole.Name);
        }

        public virtual async Task<PagedList<ApplicationRole>> GetUserRolesAsync(string userId, int page = 1, int pageSize = 10)
        {
            var id = ConvertKeyFromString(userId);

            var pagedList = new PagedList<ApplicationRole>();
            var roles = from r in DbContext.Set<ApplicationRole>()
                        join ur in DbContext.Set<ApplicationUserRole>() on r.Id equals ur.RoleId
                        where ur.UserId.Equals(id)
                        select r;

            var userIdentityRoles = await roles.PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(userIdentityRoles);
            pagedList.TotalCount = await roles.CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<IdentityResult> DeleteUserRoleAsync(string userId, string roleId)
        {
            var role = await RoleManager.FindByIdAsync(roleId);
            var user = await UserManager.FindByIdAsync(userId);

            return await UserManager.RemoveFromRoleAsync(user, role.Name);
        }

        public virtual async Task<PagedList<ApplicationUserClaim>> GetUserClaimsAsync(string userId, int page, int pageSize)
        {
            var id = ConvertKeyFromString(userId);
            var pagedList = new PagedList<ApplicationUserClaim>();

            var claims = await DbContext.Set<ApplicationUserClaim>().Where(x => x.UserId.Equals(id))
                .PageBy(x => x.UserId, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(claims);
            pagedList.TotalCount = await DbContext.Set<ApplicationUserClaim>().Where(x => x.UserId.Equals(id)).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<PagedList<ApplicationRoleClaim>> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10)
        {
            var id = ConvertKeyFromString(roleId);
            var pagedList = new PagedList<ApplicationRoleClaim>();
            var claims = await DbContext.Set<ApplicationRoleClaim>().Where(x => x.RoleId.Equals(id))
                .PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(claims);
            pagedList.TotalCount = await DbContext.Set<ApplicationRoleClaim>().Where(x => x.RoleId.Equals(id)).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<PagedList<ApplicationRoleClaim>> GetUserRoleClaimsAsync(string userId, string claimSearchText, int page = 1, int pageSize = 10)
        {
            var id = ConvertKeyFromString(userId);
            Expression<Func<ApplicationRoleClaim, bool>> searchCondition = x => x.ClaimType.Contains(claimSearchText);
            var claimsQ = DbContext.Set<ApplicationUserRole>().Where(x => x.UserId.Equals(id))
                .Join(DbContext.Set<ApplicationRoleClaim>().WhereIf(!string.IsNullOrEmpty(claimSearchText), searchCondition), ur => ur.RoleId, rc => rc.RoleId, (ur, rc) => rc);

            var claims = await claimsQ.PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            var pagedList = new PagedList<ApplicationRoleClaim>();
            pagedList.Data.AddRange(claims);
            pagedList.TotalCount = await claimsQ.CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<ApplicationUserClaim> GetUserClaimAsync(string userId, int claimId)
        {
            var userIdConverted = ConvertKeyFromString(userId);

            return DbContext.Set<ApplicationUserClaim>().Where(x => x.UserId.Equals(userIdConverted) && x.Id == claimId)
                .SingleOrDefaultAsync();
        }



        public virtual Task<ApplicationRoleClaim> GetRoleClaimAsync(string roleId, int claimId)
        {
            var roleIdConverted = ConvertKeyFromString(roleId);

            return DbContext.Set<ApplicationRoleClaim>().Where(x => x.RoleId.Equals(roleIdConverted) && x.Id == claimId)
                .SingleOrDefaultAsync();
        }



        public virtual async Task<IdentityResult> CreateUserClaimsAsync(ApplicationUserClaim claims)
        {
            var user = await UserManager.FindByIdAsync(claims.UserId.ToString());
            return await UserManager.AddClaimAsync(user, new Claim(claims.ClaimType, claims.ClaimValue));
        }

        public virtual async Task<IdentityResult> UpdateUserClaimsAsync(ApplicationUserClaim claims)
        {
            var user = await UserManager.FindByIdAsync(claims.UserId.ToString());
            var userClaim = await DbContext.Set<ApplicationUserClaim>().Where(x => x.Id == claims.Id).SingleOrDefaultAsync();

            await UserManager.RemoveClaimAsync(user, new Claim(userClaim.ClaimType, userClaim.ClaimValue));

            return await UserManager.AddClaimAsync(user, new Claim(claims.ClaimType, claims.ClaimValue));
        }

        public virtual async Task<IdentityResult> CreateRoleClaimsAsync(ApplicationRoleClaim claims)
        {
            var role = await RoleManager.FindByIdAsync(claims.RoleId.ToString());
            return await RoleManager.AddClaimAsync(role, new Claim(claims.ClaimType, claims.ClaimValue));
        }

        public virtual async Task<IdentityResult> UpdateRoleClaimsAsync(ApplicationRoleClaim claims)
        {
            var role = await RoleManager.FindByIdAsync(claims.RoleId.ToString());
            var userClaim = await DbContext.Set<ApplicationUserClaim>().Where(x => x.Id == claims.Id).SingleOrDefaultAsync();

            await RoleManager.RemoveClaimAsync(role, new Claim(userClaim.ClaimType, userClaim.ClaimValue));

            return await RoleManager.AddClaimAsync(role, new Claim(claims.ClaimType, claims.ClaimValue));
        }


        public virtual async Task<IdentityResult> DeleteUserClaimAsync(string userId, int claimId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var userClaim = await DbContext.Set<ApplicationUserClaim>().Where(x => x.Id == claimId).SingleOrDefaultAsync();

            return await UserManager.RemoveClaimAsync(user, new Claim(userClaim.ClaimType, userClaim.ClaimValue));
        }

        public virtual async Task<IdentityResult> DeleteRoleClaimAsync(string roleId, int claimId)
        {
            var role = await RoleManager.FindByIdAsync(roleId);
            var roleClaim = await DbContext.Set<ApplicationRoleClaim>().Where(x => x.Id == claimId).SingleOrDefaultAsync();

            return await RoleManager.RemoveClaimAsync(role, new Claim(roleClaim.ClaimType, roleClaim.ClaimValue));
        }

        public virtual async Task<List<UserLoginInfo>> GetUserProvidersAsync(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var userLoginInfos = await UserManager.GetLoginsAsync(user);

            return userLoginInfos.ToList();
        }

        public virtual Task<IdentityUserLogin<Guid>> GetUserProviderAsync(string userId, string providerKey)
        {
            var userIdConverted = ConvertKeyFromString(userId);

            return DbContext.Set<IdentityUserLogin<Guid>>().Where(x => x.UserId.Equals(userIdConverted) && x.ProviderKey == providerKey)
                .SingleOrDefaultAsync();
        }

        public virtual async Task<IdentityResult> DeleteUserProvidersAsync(string userId, string providerKey, string loginProvider)
        {
            var userIdConverted = ConvertKeyFromString(userId);

            var user = await UserManager.FindByIdAsync(userId);
            var login = await DbContext.Set<IdentityUserLogin<Guid>>().Where(x => x.UserId.Equals(userIdConverted) && x.ProviderKey == providerKey && x.LoginProvider == loginProvider).SingleOrDefaultAsync();
            return await UserManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
        }

        public virtual async Task<IdentityResult> UserChangePasswordAsync(string userId, string password)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var token = await UserManager.GeneratePasswordResetTokenAsync(user);

            return await UserManager.ResetPasswordAsync(user, token, password);
        }

        public virtual async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var userIdentity = await UserManager.FindByIdAsync(userId);

            return await UserManager.DeleteAsync(userIdentity);
        }

        protected virtual async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }

        public virtual async Task<int> SaveAllChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }

        //public Task<IdentityResult> CreateRoleClaimsAsync(IdentityRoleClaim<Guid> claims)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IdentityResult> UpdateRoleClaimsAsync(IdentityRoleClaim<Guid> claims)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<PagedList<IdentityRoleClaim<Guid>>> IIdentityRepository.GetRoleClaimsAsync(string roleId, int page, int pageSize)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<PagedList<IdentityRoleClaim<Guid>>> IIdentityRepository.GetUserRoleClaimsAsync(string userId, string claimSearchText, int page, int pageSize)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<IdentityRoleClaim<Guid>> IIdentityRepository.GetRoleClaimAsync(string roleId, int claimId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}