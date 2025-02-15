

using MultiAppServer.ServiceDefaults;
using Google.Protobuf.WellKnownTypes;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Polly;
using Microsoft.AspNetCore.Components.Forms;
using AutoMapper;
using System.ComponentModel.Design;
using BlazorApiUser.Commands.Users;
using BlazorApiUser.Queries.Users;
using BlazorApiUser.Queries.Roles;
using BlazorApiUser.Models;

public static class RolesApi
{
    public static RouteGroupBuilder MapRolesApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/admin").HasApiVersion(1.0);

        api.MapGet("/roles", GetRoles);

        api.MapGet("/roles/{id}", GetRoleById);

        api.MapPost("/roles", Create);

        api.MapDelete("/roles/{id}", Delete);

        api.MapPut("/roles/{id}", Update);
        return api;
    }

    [Authorize(Roles = Permissions.Role.Read)]
    public static async Task<ApiResponseDto<List<RoleDto>>> GetRoles([AsParameters] GetListRoleQuery getListRoleQuery, [AsParameters] UserServices userServices)
    {

        var userLst = await userServices.Mediator.Send(getListRoleQuery);

        return new ApiResponseDto<List<RoleDto>>(200, $"{userLst.Item1} users fetched", userLst.Item2, userLst.Item1);
    }

    [Authorize(Roles = Permissions.Role.Update)]
    public static async Task<ApiResponseDto> Update(string id, CreateRoleCommand command, [AsParameters] UserServices userServices)
    {

        if (await userServices.RoleManager.FindByIdAsync(id) == null)
        {
            return new ApiResponseDto(400, $"Role doesn't exist!!!!");
        }

        var role = await userServices.RoleManager.FindByIdAsync(id);

        if (role!.Name != command.Name)
        {
            if (await userServices.RoleManager.FindByNameAsync(command.Name) != null)
            {
                return new ApiResponseDto(400, $"Name is exist!!!!");
            }
        }

        role.Name = command.Name;
        role.Description = command.Description;

        var result = await userServices.RoleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return new ApiResponseDto(400, $"{string.Join(";", result.Errors)}");
        }

        return new ApiResponseDto(200, $"Update Success!!!!");
    }

    [Authorize(Roles = Permissions.Role.Update)]
    public static async Task<ApiResponseDto> UpdatePermission(string id, CreateRoleCommand command, [AsParameters] UserServices userServices)
    {

        if (await userServices.RoleManager.FindByNameAsync(command.Name) != null)
        {
            return new ApiResponseDto(400, $"Name is exist!!!!");
        }

        if (await userServices.RoleManager.FindByIdAsync(id) == null)
        {
            return new ApiResponseDto(400, $"Role doesn't exist!!!!");
        }

        var role = await userServices.RoleManager.FindByIdAsync(id);
        role.Name = command.Name;
        role.Description = command.Description;

        var result = await userServices.RoleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return new ApiResponseDto(400, $"{string.Join(";", result.Errors)}");
        }

        return new ApiResponseDto(200, $"Update Success!!!!");
    }

    [Authorize(Roles = Permissions.Role.Update)]
    public static async Task<ApiResponseDto<RoleDto>> GetRoleById(string id, [AsParameters] UserServices userServices)
    {

        var identityRole = await userServices.RoleManager.FindByIdAsync(id);

        if (identityRole == null)
        {
            return new ApiResponseDto(404, "Role not found!!!", null);
        }

        var claims = await userServices.RoleManager.GetClaimsAsync(identityRole);
        var permissions = claims.OrderBy(x => x.Value).Where(x => x.Type == ApplicationClaimTypes.Permission).Select(x => x.Value).ToList();

        var roleDto = new RoleDto
        {
            Name = identityRole.Name
        };

        return new ApiResponseDto(200, "Role fetched", roleDto);
    }

    [Authorize(Roles = Permissions.Role.Create)]
    public static async Task<ApiResponseDto> Create(CreateRoleCommand command, [AsParameters] UserServices userServices)
    {
        var sendCommand = await userServices.Mediator.Send(command);

        return new ApiResponseDto(sendCommand.Item1, sendCommand.Item2, command);
    }

    [Authorize(Roles = Permissions.Role.Delete)]
    public static async Task<ApiResponseDto> Delete(string id, [AsParameters] UserServices userServices)
    {
        var roleDelete = await userServices.RoleManager.FindByIdAsync(id);

        if (roleDelete == null)
        {
            return new ApiResponse(400, "Role doesn't exit!!!", null);

        }

        if ((await userServices.UserManager.GetUsersInRoleAsync(roleDelete.Name)).Any())
        {
            return new ApiResponseDto(400, "Role map with user, can't delete!!!", null);
        }

        var sendCommand = await userServices.RoleManager.DeleteAsync(roleDelete);

        if (!sendCommand.Succeeded)
        {
            return new ApiResponseDto(400, string.Join(";", sendCommand.Errors), null);

        }

        return new ApiResponseDto(200, "Role fetched", null);

    }

    public static async Task<ApiResponseDto<UserRolesResponse>> GetUserByRoleId([FromQuery] string id, [AsParameters] UserServices userServices)
    {

        var viewModel = new List<UserRoleModel>();
        var user = await userServices.UserManager.FindByIdAsync(id);

        if (user == null)
        {
            return new ApiResponseDto<UserRolesResponse>(200, "Success", null);
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
        return new ApiResponseDto<UserRolesResponse>(200, "Success", result);
    }
}
