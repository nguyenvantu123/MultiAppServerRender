

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
        var api = app.MapGroup("api/admin").HasApiVersion(1.0);

        api.MapGet("/roles", GetRoles);

        api.MapGet("/roles/{id}", GetRoleById);

        api.MapPost("/roles/create", Create);

        api.MapDelete("/roles/{id}", Delete);

        api.MapPost("/roles/{id}", Update);
        return api;
    }

    [Authorize(Roles = Permissions.Role.Read)]
    public static async Task<ApiResponse<List<RoleDto>>> GetRoles([AsParameters] GetListRoleQuery getListRoleQuery, [AsParameters] UserServices userServices)
    {

        var userLst = await userServices.Mediator.Send(getListRoleQuery);

        return new ApiResponse<List<RoleDto>>(200, $"{userLst.Item1} users fetched", userLst.Item2, userLst.Item1);
    }

    [Authorize(Roles = Permissions.Role.Update)]
    public static async Task<ApiResponse> Update(string id, [FromBody] CreateRoleCommand command, [AsParameters] RoleManager<ApplicationRole> roleManager)
    {

        if (await roleManager.FindByNameAsync(command.Name) != null)
        {
            return new ApiResponse(400, $"Name is exist!!!!");
        }

        if (await roleManager.FindByIdAsync(id) == null)
        {
            return new ApiResponse(400, $"Role doesn't exist!!!!");
        }

        var role = await roleManager.FindByIdAsync(id);
        role.Name = command.Name;
        role.Description = command.Description;

        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return new ApiResponse(400, $"{string.Join(";", result.Errors)}");
        }

        return new ApiResponse(200, $"Update Success!!!!");
    }

    [Authorize(Roles = Permissions.Role.Update)]
    public static async Task<ApiResponse> UpdatePermission(string id, [FromBody] CreateRoleCommand command, [AsParameters] RoleManager<ApplicationRole> roleManager)
    {

        if (await roleManager.FindByNameAsync(command.Name) != null)
        {
            return new ApiResponse(400, $"Name is exist!!!!");
        }

        if (await roleManager.FindByIdAsync(id) == null)
        {
            return new ApiResponse(400, $"Role doesn't exist!!!!");
        }

        var role = await roleManager.FindByIdAsync(id);
        role.Name = command.Name;
        role.Description = command.Description;

        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return new ApiResponse(400, $"{string.Join(";", result.Errors)}");
        }

        return new ApiResponse(200, $"Update Success!!!!");
    }

    [Authorize(Roles = Permissions.Role.Update)]
    public static async Task<ApiResponse<RoleDto>> GetRoleById(string id, [AsParameters] RoleManager<ApplicationRole> roleManager, [AsParameters] EntityPermissions entityPermissions)
    {

        var identityRole = await roleManager.FindByIdAsync(id);

        if (identityRole == null)
        {
            return new ApiResponse(404, "Role not found!!!", null);
        }

        var claims = await roleManager.GetClaimsAsync(identityRole);
        var permissions = claims.OrderBy(x => x.Value).Where(x => x.Type == ApplicationClaimTypes.Permission).Select(x => entityPermissions.GetPermissionByValue(x.Value).Name).ToList();

        var roleDto = new RoleDto
        {
            Name = identityRole.Name,
        };

        return new ApiResponse(200, "Role fetched", roleDto);
    }

    [Authorize(Roles = Permissions.Role.Create)]
    public static async Task<ApiResponse> Create([FromBody] CreateRoleCommand command, IMediator mediator)
    {
        var sendCommand = await mediator.Send(command);

        return new ApiResponse(sendCommand.Item1, sendCommand.Item2, command);
    }

    [Authorize(Roles = Permissions.Role.Delete)]
    public static async Task<ApiResponse> Delete(string id, [AsParameters] UserManager<ApplicationUser> userManager, [AsParameters] RoleManager<ApplicationRole> roleManager)
    {
        var roleDelete = await roleManager.FindByIdAsync(id);

        if (roleDelete == null)
        {
            return new ApiResponse(400, "Role doesn't exit!!!", null);

        }

        if ((await userManager.GetUsersInRoleAsync(roleDelete.Name)).Any())
        {
            return new ApiResponse(400, "Role map with user, can't delete!!!", null);
        }

        var sendCommand = await roleManager.DeleteAsync(roleDelete);

        if (!sendCommand.Succeeded)
        {
            return new ApiResponse(400, string.Join(";", sendCommand.Errors), null);

        }

        return new ApiResponse(200, "Role fetched", null);

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
}
