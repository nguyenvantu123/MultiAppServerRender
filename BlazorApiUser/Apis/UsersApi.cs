

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

public static class UsersApi
{
    public static RouteGroupBuilder MapUsersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/admin").HasApiVersion(1.0);

        api.MapPost("/users/create", Create);

        api.MapPost("/users/update-user", AdminUpdateUser);

        //api.MapDelete("/users/delete-user/{id}", AdminUpdateUser);

        //api.MapPost("/users/reset-password", ResetPasswordUser);

        //api.MapGet("/users/get-permission-by-user", (UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ClaimsPrincipal userAuth) => GetPermissionByUser(userManager, userAuth));

        api.MapGet("/users", [Authorize(Roles = $"Administrator")] (int pageNumber = 0, int pageSize = 10) => GetUser).AllowAnonymous();

        //api.MapGet("/users/{id}", GetUserById);

        //api.MapGet("/users/user-roles/{roleId}", GetUserByRoleId);

        //api.MapPut("/users/update-user-roles", UpdateUserRoles);

        //api.MapPut("/users/toggle-user-status", ToggleUserStatus);

        return api;
    }

    public static async Task<ApiResponse> ToggleUserStatus([FromBody] ToogleUserRequestCommand toggleUserStatusCommand, [AsParameters] UserServices userServices)
    {
        var sendCommand = await userServices.Mediator.Send(toggleUserStatusCommand);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, sendCommand);
    }

    public static async Task<ApiResponse> Create([FromBody] CreateUserCommand command, [AsParameters] UserServices userServices)
    {
        var sendCommand = await userServices.Mediator.Send(command);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, command);
    }

    public static async Task<ApiResponse> AdminUpdateUser([FromBody] AdminUpdateUserCommand command, [AsParameters] UserServices userServices)
    {
        var sendCommand = await userServices.Mediator.Send(command);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, command);
    }

    public static async Task<ApiResponse> AdminUpdateDelete([FromQuery] string id, [AsParameters] UserServices userServices)
    {
        AdminDeleteUserCommand adminDeleteUserCommand = new AdminDeleteUserCommand();
        adminDeleteUserCommand.Id = id;

        var sendCommand = await userServices.Mediator.Send(adminDeleteUserCommand);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, null);
    }

    public static async Task<ApiResponse> ResetPasswordUser([FromBody] ChangePasswordCommand changePasswordCommand, [AsParameters] UserServices userServices)
    {

        var sendCommand = await userServices.Mediator.Send(changePasswordCommand);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, null);
    }

    public static async Task<ApiResponse<List<string>>> GetPermissionByUser(UserManager<ApplicationUser> userManager, ClaimsPrincipal userAuth)
    {

        var user = await userManager.GetUserAsync(userAuth);

        if (user != null)
        {
            var claims = await userManager.GetClaimsAsync(user);

            if (claims != null)
            {
                return new ApiResponse<List<string>>(statusCode: 200, message: "", result: claims.ToList().Select(x => x.Value).ToList());

            }
            return new ApiResponse<List<string>>(statusCode: 200, message: "", result: new List<string>());
        }

        return new ApiResponse<List<string>>(statusCode: 200, message: "", result: new List<string>());
    }

    public static async Task<ApiResponse<List<UserDataViewModel>>> GetUser([FromServices] UserServices userServices)
    {
        GetListUserQuery getListUserQuery = new GetListUserQuery();
        getListUserQuery.PageNumber = 0;
        getListUserQuery.PageSize = 10;
        var userLst = await userServices.Mediator.Send(getListUserQuery);

        return new ApiResponse<List<UserDataViewModel>>(200, $"{userLst.Item1} users fetched", userLst.Item2, userLst.Item1);
    }

    public static async Task<ApiResponse<UserDataViewModel>> GetUserById([AsParameters] string id, UserManager<ApplicationUser> userManager, IMapper autoMapper)
    {

        var user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            return new ApiResponse<UserDataViewModel>(404, "User Not Found", null);
        }

        var result = autoMapper.Map<UserDataViewModel>(user);

        return new ApiResponse<UserDataViewModel>(200, "Success", result);
    }

    public static async Task<ApiResponse<UserRolesResponse>> GetUserByRoleId([AsParameters] string id, [AsParameters] UserManager<ApplicationUser> userManager, [AsParameters] RoleManager<ApplicationRole> roleManager, IMapper autoMapper)
    {

        var viewModel = new List<UserRoleModel>();
        var user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            return new ApiResponse<UserRolesResponse>(200, "Success", null);
        }
        var roles = await roleManager.Roles.ToListAsync();

        foreach (var role in roles)
        {
            var userRolesViewModel = new UserRoleModel
            {
                RoleName = role.Name,
                RoleDescription = role.Description
            };
            if (await userManager.IsInRoleAsync(user, role.Name))
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

    public static async Task<ApiResponse> UpdateUserRoles([FromBody] UpdateUserRolesCommand updateUserRolesCommand, [AsParameters] IMediator mediator)
    {

        var command = await mediator.Send(updateUserRolesCommand);
        return new ApiResponse(command.Item1, command.Item2, null);
    }
}
