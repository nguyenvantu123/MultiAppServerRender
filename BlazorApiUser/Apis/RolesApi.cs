

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

public static class RolesApi
{
    public static RouteGroupBuilder MapRolesApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/roles").HasApiVersion(1.0);

        api.MapGet("/admin/roles", GetRoles);

        api.MapGet("/admin/get-role/{name}", [Authorize] () => GetRoleByName);

        api.MapPost("/admin/create", [Authorize(Roles = "Administrator", Policy = Permissions.Role.Create)] () => Create).AllowAnonymous();

        //api.MapPost("/admin/update-user", [Authorize(Roles = "Administrator", Policy = Permissions.User.Update)] () => AdminUpdateUser);

        //api.MapDelete("/admin/delete-user/{id}", [Authorize(Roles = "Administrator", Policy = Permissions.User.Delete)] () => AdminUpdateUser);

        //api.MapPost("/admin/reset-password", [Authorize(Roles = "Administrator", Policy = Permissions.User.Update)] () => ResetPasswordUser);

        //api.MapGet("/admin/get-permission-by-user", [Authorize] (UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ClaimsPrincipal userAuth) => GetPermissionByUser(userManager, userAuth));


        //api.MapGet("/admin/user/{id}", [Authorize(Roles = "Administrator", Policy = Permissions.User.Read)] () => GetUserById);

        //api.MapGet("/admin/user-roles/{roleId}", [Authorize(Roles = "Administrator", Policy = Permissions.User.Read)] () => GetUserByRoleId);

        //api.MapPut("/admin/update-user-roles", [Authorize(Roles = "Administrator", Policy = Permissions.User.Update)] () => UpdateUserRoles);

        //api.MapPut("/admin/toggle-user-status", [Authorize(Roles = "Administrator", Policy = Permissions.User.Update)] () => ToggleUserStatus);

        //api.MapGet("{orderId:int}", GetOrderAsync);
        //api.MapGet("/", GetOrdersByUserAsync);
        //api.MapGet("/cardtypes", GetCardTypesAsync);
        //api.MapPost("/draft", CreateOrderDraftAsync);
        //api.MapPost("/", CreateOrderAsync);

        return api;
    }

    public static async Task<ApiResponse<List<RoleDto>>> GetRoles([AsParameters] GetListRoleQuery getListRoleQuery, [AsParameters] UserServices userServices)
    {

        var userLst = await userServices.Mediator.Send(getListRoleQuery);

        return new ApiResponse<List<RoleDto>>(200, $"{userLst.Item1} users fetched", userLst.Item2, userLst.Item1);
    }

    public static async Task<ApiResponse<List<RoleDto>>> GetRoleByName(string name, [AsParameters] RoleManager<ApplicationRole> roleManager, [AsParameters] EntityPermissions entityPermissions)
    {

        var identityRole = await roleManager.FindByNameAsync(name);

        if (identityRole == null)
        {
            return new ApiResponse(404, "Role not found!!!", null);
        }

        var claims = await roleManager.GetClaimsAsync(identityRole);
        var permissions = claims.OrderBy(x => x.Value).Where(x => x.Type == ApplicationClaimTypes.Permission).Select(x => entityPermissions.GetPermissionByValue(x.Value).Name).ToList();

        var roleDto = new RoleDto
        {
            Name = name,
        };

        return new ApiResponse(200, "Role fetched", roleDto);
    }

    public static async Task<ApiResponse> Create([FromBody] CreateRoleCommand command, IMediator mediator)
    {
        var sendCommand = await mediator.Send(command);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, command);
    }




    public static async Task<ApiResponse> ToggleUserStatus([FromBody] ToogleUserRequestCommand toggleUserStatusCommand, IMediator mediator)
    {
        var sendCommand = await mediator.Send(toggleUserStatusCommand);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, sendCommand);
    }


    public static async Task<ApiResponse> AdminUpdateUser([FromBody] AdminUpdateUserCommand command, IMediator mediator)
    {
        var sendCommand = await mediator.Send(command);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, command);
    }

    public static async Task<ApiResponse> AdminUpdateDelete([FromQuery] string id, IMediator mediator)
    {
        AdminDeleteUserCommand adminDeleteUserCommand = new AdminDeleteUserCommand();
        adminDeleteUserCommand.Id = id;

        var sendCommand = await mediator.Send(adminDeleteUserCommand);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, null);
    }

    public static async Task<ApiResponse> ResetPasswordUser([FromBody] ChangePasswordCommand changePasswordCommand, IMediator mediator)
    {

        var sendCommand = await mediator.Send(changePasswordCommand);

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

    public static async Task<ApiResponse<UserDataViewModel>> GetUserById([FromQuery] string id, [AsParameters] UserServices userServices, UserManager<ApplicationUser> userManager, IMapper autoMapper)
    {

        var user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            return new ApiResponse<UserDataViewModel>(404, "User Not Found", null);
        }

        var result = autoMapper.Map<UserDataViewModel>(user);

        return new ApiResponse<UserDataViewModel>(200, "Success", result);
    }

    public static async Task<ApiResponse<UserRolesResponse>> GetUserByRoleId([FromQuery] string id, [AsParameters] UserServices userServices, [AsParameters] UserManager<ApplicationUser> userManager, [AsParameters] RoleManager<ApplicationRole> roleManager, IMapper autoMapper)
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

    public static async Task<ApiResponse> UpdateUserRoles([FromBody] UpdateUserRolesCommand updateUserRolesCommand, IMediator mediator)
    {

        var command = await mediator.Send(updateUserRolesCommand);
        return new ApiResponse(command.Item1, command.Item2, null);
    }
}
