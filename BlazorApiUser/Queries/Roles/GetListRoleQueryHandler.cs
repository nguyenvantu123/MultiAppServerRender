
// Regular CommandHandler
using AutoMapper;
using BlazorApiUser.Queries.Roles;
using BlazorApiUser.Repository;
using BlazorBoilerplate.Constants;
using BlazorIdentity.Users;
using BlazorIdentity.Users.Extensions;
using BlazorIdentity.Users.Models;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiAppServer.ServiceDefaults;
using System.Net;
using System.Security.Claims;
using WebApp.Models;


namespace BlazorApiUser.Queries.Users;

public class GetListRoleQueryHandler : IRequestHandler<GetListRoleQuery, Tuple<int, List<RoleDto>>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    //private readonly UserManager<ApplicationUser> _userManager;
    private readonly EntityPermissions _entityPermissions;


    public GetListRoleQueryHandler(RoleManager<ApplicationRole> roleManager, EntityPermissions entityPermissions)
    {
        _roleManager = roleManager;
        _entityPermissions = entityPermissions;
    }

    /// <summary>
    /// Handler which processes the command when
    /// customer executes cancel order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<Tuple<int, List<RoleDto>>> Handle(GetListRoleQuery command, CancellationToken cancellationToken)
    {
        var roleQuery = _roleManager.Roles.AsQueryable().OrderBy(x => x.Name);
        var count = roleQuery.Count();
        var listResponse = (command.pageSize > 0 ? roleQuery.Skip(command.pageNumber * command.pageSize).Take(command.pageSize) : roleQuery).ToList();

        var roleDtoList = new List<RoleDto>();

        foreach (var role in listResponse)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            roleDtoList.Add(new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
                //Permissions = permissions
            });
        }


        return new Tuple<int, List<RoleDto>>(count, roleDtoList);

        //return new Tuple<int, string>(200, $"User {user.UserName} deleted");
    }
}

