

using MultiAppServer.ServiceDefaults;
using Google.Protobuf.WellKnownTypes;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using BlazorIdentity.Users.Models;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using static BlazorIdentity.Users.Models.Permissions;
using Microsoft.AspNetCore.Authorization;
using Polly;
using Microsoft.AspNetCore.Components.Forms;
using AutoMapper;
using System.ComponentModel.Design;
using BlazorApiUser.Commands.Users;
using BlazorApiUser.Queries.Users;
using BlazorApiUser.Queries.Roles;
using WebApp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BlazorIdentity.Users.Constants;

public static class UsersApi
{
    public static RouteGroupBuilder MapUsersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/admin").HasApiVersion(1.0);

        api.MapGet("/users", GetUsers);

        api.MapPost("/users", Create);

        api.MapPut("/users/{id}", AdminUpdateUser);

        api.MapDelete("/users/{id}", AdminDelete);

        api.MapPost("/users/reset-password", ResetPasswordUser);

        api.MapGet("/users/get-permission-by-user", GetPermissionByUser);

        api.MapGet("/users/{id}", GetUserById);

        api.MapGet("/users/user-roles/{id}", GetRoleByUserId);

        api.MapPut("/users/update-user-roles/{id}", UpdateUserRoles);

        api.MapPut("/users/toggle-user-status", ToggleUserStatus);

        api.MapGet("/users/user-view-model", UserViewModel);

        return api;
    }

    [Authorize(Roles = Permissions.User.Read)]
    public static async Task<ApiResponse<List<UserDataViewModel>>> GetUsers([AsParameters] GetListUserQuery getListUserQuery, [AsParameters] UserServices userServices)
    {
        var userLst = await userServices.Mediator.Send(getListUserQuery);

        return new ApiResponse<List<UserDataViewModel>>(200, $"{userLst.Item1} users fetched", userLst.Item2, userLst.Item1);
    }

    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponse> ToggleUserStatus(ToogleUserRequestCommand toggleUserStatusCommand, [AsParameters] UserServices userServices)
    {
        var sendCommand = await userServices.Mediator.Send(toggleUserStatusCommand);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, sendCommand);
    }

    [Authorize(Roles = Permissions.User.Create)]
    public static async Task<ApiResponse> Create(CreateUserCommand command, [AsParameters] UserServices userServices)
    {
        var sendCommand = await userServices.Mediator.Send(command);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, command);
    }

    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponse> AdminUpdateUser(string id, AdminUpdateUserCommand command, [AsParameters] UserServices userServices)
    {
        //var sendCommand = await userServices.Mediator.Send(command);

        var user = await userServices.UserManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ApiResponse(400, "User not found!!!", null);
        }

        if (user.UserName == "admin")
        {
            return new ApiResponse(400, "Can't update admin user!!!", null);
        }

        //user.UserName = command.UserName;

        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.PhoneNumber = command.PhoneNumber;

        var result = await userServices.UserManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return new ApiResponse(400, string.Join(";", result.Errors), null);
        }

        return new ApiResponse(200, $"{command.UserName} update successfully!!!", null);
    }

    [Authorize(Roles = Permissions.User.Delete)]
    public static async Task<ApiResponse> AdminDelete(string id, [AsParameters] UserServices userServices)
    {

        var user = await userServices.UserManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ApiResponse(400, "User not found!!!", null);
        }

        if (user.UserName == "Administrator")
        {
            return new ApiResponse(400, "Can't delete admin user!!!", null);
        }

        var result = await userServices.UserManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return new ApiResponse(400, string.Join(";", result.Errors), null);

        }

        return new ApiResponse(200, $"{user.UserName} delete successfully!!!", null);
    }

    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponse> ResetPasswordUser(ChangePasswordCommand changePasswordCommand, [AsParameters] UserServices userServices)
    {

        var sendCommand = await userServices.Mediator.Send(changePasswordCommand);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, null);
    }

    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponse<List<string>>> GetPermissionByUser([AsParameters] UserServices userServices, ClaimsPrincipal userAuth)
    {

        var user = await userServices.UserManager.GetUserAsync(userAuth);

        if (user != null)
        {
            var claims = await userServices.UserManager.GetClaimsAsync(user);

            if (claims != null)
            {
                return new ApiResponse<List<string>>(statusCode: 200, message: "", result: claims.ToList().Select(x => x.Value).ToList());

            }
            return new ApiResponse<List<string>>(statusCode: 200, message: "", result: new List<string>());
        }

        return new ApiResponse<List<string>>(statusCode: 200, message: "", result: new List<string>());
    }

    //public static async Task<ApiResponse<List<UserDataViewModel>>> GetUser([FromServices] UserServices userServices)
    //{

    //}
    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponse<UserDataViewModel>> GetUserById(string id, [AsParameters] UserServices userServices, IMapper autoMapper)
    {

        var user = await userServices.UserManager.FindByIdAsync(id);

        if (user == null)
        {
            return new ApiResponse<UserDataViewModel>(404, "User Not Found", null);
        }

        var result = autoMapper.Map<UserDataViewModel>(user);

        return new ApiResponse<UserDataViewModel>(200, "Success", result);
    }
    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponse<UserRolesResponse>> GetRoleByUserId(string id, [AsParameters] UserServices userServices, IMapper autoMapper)
    {

        var viewModel = new List<UserRoleModel>();
        var user = await userServices.UserManager.FindByIdAsync(id);

        if (user == null)
        {
            return new ApiResponse<UserRolesResponse>(200, "Success", null);
        }
        var roles = await userServices.RoleManager.Roles.ToListAsync();

        foreach (var role in roles)
        {
            var userRolesViewModel = new UserRoleModel
            {
                RoleName = role.Name,
                RoleDescription = role.Description
            };
            if (await userServices.UserManager.IsInRoleAsync(user, role.Name))
            {
                userRolesViewModel.Selected = true;
            }
            else
            {
                userRolesViewModel.Selected = false;
            }
            viewModel.Add(userRolesViewModel);
        }
        var result = new UserRolesResponse { UserRoles = viewModel };
        return new ApiResponse<UserRolesResponse>(200, "Success", result);
    }
    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponse> UpdateUserRoles(string id, UpdateUserRolesCommand updateUserRolesCommand, [AsParameters] UserServices userServices)
    {
        var user = await userServices.UserManager.FindByIdAsync(id);

        if (user == null)
        {
            return new ApiResponse(400, "Not found user!!!");
        }

        if (user.UserName == DefaultUserNames.Administrator)
        {
            return new ApiResponse(400, "Not Allowed");
        }

        var userAccessor = userServices.HttpContextAccessor!.HttpContext!.User;
        var userId = new Guid(userAccessor.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

        var roles = await userServices.UserManager.GetRolesAsync(user);
        var selectedRoles = updateUserRolesCommand.UserRoles.Where(x => x.Selected).ToList();

        var currentUser = await userServices.UserManager.FindByIdAsync(userId.ToString());

        if (currentUser == null)
        {
            return new ApiResponse(401, "Unauthorized!!!");
        }

        if (!await userServices.UserManager.IsInRoleAsync(currentUser, DefaultRoleNames.Administrator))
        {
            var tryToAddAdministratorRole = selectedRoles
                .Any(x => x.RoleName == DefaultRoleNames.Administrator);
            var userHasAdministratorRole = roles.Any(x => x == DefaultRoleNames.Administrator);
            if (tryToAddAdministratorRole && !userHasAdministratorRole || !tryToAddAdministratorRole && userHasAdministratorRole)
            {

                return new ApiResponse(400, "Not Allowed to add or delete Administrator Role if you have not this role.");

            }
        }

        var result = await userServices.UserManager.RemoveFromRolesAsync(user, roles);
        result = await userServices.UserManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));

        return new ApiResponse(200, "Roles Updated");
    }

    [Authorize]
    public static async Task<ApiResponse<UserDataViewModel>> UserViewModel([AsParameters] UserServices userServices, ClaimsPrincipal userAuth, IMapper autoMapper)
    {
        var user = await userServices.UserManager.FindByIdAsync(userAuth!.FindFirstValue("sub")!);

        if (user == null)
        {
            return new ApiResponse<UserDataViewModel>(404, "User Not Found", null);
        }

        var result = autoMapper.Map<UserDataViewModel>(user);

        result.Roles = await userServices.UserManager.GetRolesAsync(user);

        return new ApiResponse<UserDataViewModel>(200, "Success", result);
    }
}
