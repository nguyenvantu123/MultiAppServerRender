// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorIdentity.Controllers;
using BlazorIdentity.Helpers.Localization;
using BlazorIdentity.Users.Constants;
using BlazorIdentityApi.Common;
using BlazorIdentityApi.Dtos.Identity;
using BlazorIdentityApi.ExceptionHandling;
using BlazorIdentityApi.Services.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;

namespace BlazorIdentity.Controllers
{
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    public class IdentityController : BaseController
    {
        private readonly IIdentityService _identityService;
        private readonly IGenericControllerLocalizer<IdentityController> _localizer;
        ILogger<ConfigurationController> logger;
        public IdentityController(IIdentityService identityService,
            ILogger<ConfigurationController> logger,
           IGenericControllerLocalizer<IdentityController> localizer) : base(logger)
        {
            _identityService = identityService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Roles(int? page, string search)
        {
            ViewBag.Search = search;
            var roles = await _identityService.GetRolesAsync(search, page ?? 1);

            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> Role(Guid id)
        {
            if (EqualityComparer<Guid>.Default.Equals(id, default))
            {
                return View(new RoleDto<Guid>());
            }

            var role = await _identityService.GetRoleAsync(id.ToString());

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Role(RoleDto<Guid> role)
        {
            if (!ModelState.IsValid)
            {
                return View(role);
            }

            Guid roleId;

            if (EqualityComparer<Guid>.Default.Equals(role.Id, default))
            {
                var roleData = await _identityService.CreateRoleAsync(role);
                roleId = roleData.roleId;
            }
            else
            {
                var roleData = await _identityService.UpdateRoleAsync(role);
                roleId = roleData.roleId;
            }

            SuccessNotification(string.Format(_localizer["SuccessCreateRole"], role.Name), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(Role), new { Id = roleId });
        }

        [HttpGet]
        public async Task<IActionResult> Users(int? page, string search)
        {
            ViewBag.Search = search;
            var usersDto = await _identityService.GetUsersAsync(search, page ?? 1);

            return View(usersDto);
        }

        [HttpGet]
        public async Task<IActionResult> RoleUsers(string roleId, int? page, string search)
        {
            ViewBag.Search = search;
            var roleUsers = await _identityService.GetRoleUsersAsync(roleId, search, page ?? 1);

            var roleDto = await _identityService.GetRoleAsync(roleId);
            ViewData["RoleName"] = roleDto.Name;

            return View(roleUsers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(UserDto<Guid> user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            Guid userId;

            if (EqualityComparer<Guid>.Default.Equals(user.Id, default))
            {
                var userData = await _identityService.CreateUserAsync(user);
                userId = userData.userId;
            }
            else
            {
                var userData = await _identityService.UpdateUserAsync(user);
                userId = userData.userId;
            }

            SuccessNotification(string.Format(_localizer["SuccessCreateUser"], user.UserName), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(UserProfile), new { Id = userId });
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile(Guid id)
        {
            if (EqualityComparer<Guid>.Default.Equals(id, default))
            {
                var newUser = new UserDto<Guid>();

                return View("UserProfile", newUser);
            }

            var user = await _identityService.GetUserAsync(id.ToString());
            if (user == null) return NotFound();

            return View("UserProfile", user);
        }

        [HttpGet]
        public async Task<IActionResult> UserRoles(Guid id, int? page)
        {
            if (EqualityComparer<Guid>.Default.Equals(id, default)) return NotFound();

            var userRoles = await _identityService.BuildUserRolesViewModel(id, page);

            return View(userRoles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserRoles(UserRolesDto<RoleDto<Guid>, Guid> role)
        {
            await _identityService.CreateUserRoleAsync(role);
            SuccessNotification(_localizer["SuccessCreateUserRole"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(UserRoles), new { Id = role.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> UserRolesDelete(Guid id, Guid roleId)
        {
            await _identityService.ExistsUserAsync(id.ToString());
            await _identityService.ExistsRoleAsync(roleId.ToString());

            var userDto = await _identityService.GetUserAsync(id.ToString());
            var roles = await _identityService.GetRolesAsync();

            var rolesDto = new UserRolesDto<RoleDto<Guid>, Guid>
            {
                UserId = id,
                RolesList = roles.Select(x => new SelectItemDto(x.Id.ToString(), x.Name)).ToList(),
                RoleId = roleId,
                UserName = userDto.UserName
            };

            return View(rolesDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserRolesDelete(UserRolesDto<RoleDto<Guid>, Guid> role)
        {
            await _identityService.DeleteUserRoleAsync(role);
            SuccessNotification(_localizer["SuccessDeleteUserRole"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(UserRoles), new { Id = role.UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserClaims(UserClaimDto<Guid> claim)
        {
            if (!ModelState.IsValid)
            {
                return View(claim as UserClaimsDto<UserClaimDto<Guid>, Guid>);
            }

            await _identityService.CreateUserClaimsAsync(claim);
            SuccessNotification(string.Format(_localizer["SuccessCreateUserClaims"], claim.ClaimType, claim.ClaimValue), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(UserClaims), new { Id = claim.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> UserClaims(Guid id, int? page)
        {
            if (EqualityComparer<Guid>.Default.Equals(id, default)) return NotFound();

            var claims = await _identityService.GetUserClaimsAsync(id.ToString(), page ?? 1);
            claims.UserId = id;

            return View(claims);
        }

        [HttpGet]
        public async Task<IActionResult> UserClaimsDelete(Guid id, int claimId)
        {
            if (EqualityComparer<Guid>.Default.Equals(id, default)
            || EqualityComparer<int>.Default.Equals(claimId, default)) return NotFound();

            var claim = await _identityService.GetUserClaimAsync(id.ToString(), claimId);
            if (claim == null) return NotFound();

            var userDto = await _identityService.GetUserAsync(id.ToString());
            claim.UserName = userDto.UserName;

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserClaimsDelete(UserClaimDto<Guid> claim)
        {
            await _identityService.DeleteUserClaimAsync(claim);
            SuccessNotification(_localizer["SuccessDeleteUserClaims"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(UserClaims), new { Id = claim.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> UserProviders(Guid id)
        {
            if (EqualityComparer<Guid>.Default.Equals(id, default)) return NotFound();

            var providers = await _identityService.GetUserProvidersAsync(id.ToString());

            return View(providers);
        }

        //[HttpGet]
        //public async Task<IActionResult> UserProvidersDelete(TKey id, string providerKey)
        //{
        //    if (EqualityComparer<Guid>.Default.Equals(id, default) || string.IsNullOrEmpty(providerKey)) return NotFound();

        //    var provider = await _identityService.GetUserProviderAsync(id.ToString(), providerKey);
        //    if (provider == null) return NotFound();

        //    return View(provider);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProvidersDelete(UserProviderDto<Guid> provider)
        {
            await _identityService.DeleteUserProvidersAsync(provider);
            SuccessNotification(_localizer["SuccessDeleteUserProviders"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(UserProviders), new { Id = provider.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> UserChangePassword(Guid id)
        {
            if (EqualityComparer<Guid>.Default.Equals(id, default)) return NotFound();

            var user = await _identityService.GetUserAsync(id.ToString());
            var userDto = new UserChangePasswordDto<Guid> { UserId = id, UserName = user.UserName };

            return View(userDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserChangePassword(UserChangePasswordDto<Guid> userPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(userPassword);
            }

            var identityResult = await _identityService.UserChangePasswordAsync(userPassword);

            if (!identityResult.Errors.Any())
            {
                SuccessNotification(_localizer["SuccessUserChangePassword"], _localizer["SuccessTitle"]);

                return RedirectToAction("UserProfile", new { Id = userPassword.UserId });
            }

            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(userPassword);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleClaims(RoleClaimDto<Guid> claim)
        {
            if (!ModelState.IsValid)
            {
                return View((RoleClaimsDto<RoleClaimDto<Guid>, Guid>)claim);
            }

            await _identityService.CreateRoleClaimsAsync(claim);
            SuccessNotification(string.Format(_localizer["SuccessCreateRoleClaims"], claim.ClaimType, claim.ClaimValue), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(RoleClaims), new { Id = claim.RoleId });
        }

        [HttpGet]
        public async Task<IActionResult> RoleClaims(Guid id, int? page)
        {
            if (EqualityComparer<Guid>.Default.Equals(id, default)) return NotFound();

            var claims = await _identityService.GetRoleClaimsAsync(id.ToString(), page ?? 1);
            claims.RoleId = id;

            return View(claims);
        }

        [HttpGet]
        public async Task<IActionResult> RoleClaimsDelete(Guid id, int claimId)
        {
            if (EqualityComparer<Guid>.Default.Equals(id, default) ||
                EqualityComparer<int>.Default.Equals(claimId, default)) return NotFound();

            var claim = await _identityService.GetRoleClaimAsync(id.ToString(), claimId);

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleClaimsDelete(RoleClaimDto<Guid> claim)
        {
            await _identityService.DeleteRoleClaimAsync(claim);
            SuccessNotification(_localizer["SuccessDeleteRoleClaims"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(RoleClaims), new { Id = claim.RoleId });
        }

        [HttpGet]
        public async Task<IActionResult> RoleDelete(Guid id)
        {
            if (EqualityComparer<Guid>.Default.Equals(id, default)) return NotFound();

            var roleDto = await _identityService.GetRoleAsync(id.ToString());
            if (roleDto == null) return NotFound();

            return View(roleDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleDelete(RoleDto<Guid> role)
        {
            await _identityService.DeleteRoleAsync(role);
            SuccessNotification(_localizer["SuccessDeleteRole"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(Roles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDelete(UserDto<Guid> user)
        {
            var currentUserId = User.GetSubjectId();
            if (user.Id.ToString() == currentUserId)
            {
                CreateNotification(BlazorIdentity.Helpers.NotificationHelpers.AlertType.Warning, _localizer["ErrorDeleteUser_CannotSelfDelete"]);
                return RedirectToAction(nameof(UserDelete), user.Id);
            }
            else
            {
                await _identityService.DeleteUserAsync(user.Id.ToString(), user);
                SuccessNotification(_localizer["SuccessDeleteUser"], _localizer["SuccessTitle"]);

                return RedirectToAction(nameof(Users));
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserDelete(Guid id)
        {
            if (EqualityComparer<Guid>.Default.Equals(id, default)) return NotFound();

            var user = await _identityService.GetUserAsync(id.ToString());
            if (user == null) return NotFound();

            return View(user);
        }
    }
}