
// Regular CommandHandler
using BlazorApiUser.Repository;
using BlazorBoilerplate.Constants;
using BlazorIdentity.Users;
using BlazorIdentity.Users.Extensions;
using BlazorIdentity.Users.Models;
using BlazorIdentity.Users.Models.Email;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;
using WebApp.Models;


namespace BlazorApiUser.Commands.Users;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Tuple<int, string>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public CreateRoleCommandHandler( RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    /// <summary>
    /// Handler which processes the command when
    /// customer executes cancel order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<Tuple<int, string>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {

        if (_roleManager.Roles.Any(r => r.Name == command.Name))
        {
            return new Tuple<int, string>(400, $"Role {command.Name} already exists");
        }

        var result = await _roleManager.CreateAsync(new ApplicationRole(command.Name));

        if (!result.Succeeded)
        {
            var msg = result.GetErrors();
            return new Tuple<int, string>(400, msg);
        }

        // Re-create the permissions
        var role = await _roleManager.FindByNameAsync(command.Name);

        //foreach (var claim in roleDto.Permissions)
        //{
        //    var resultAddClaim = await _roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, _entityPermissions.GetPermissionByName(claim)));

        //    if (!resultAddClaim.Succeeded)
        //        await _roleManager.DeleteAsync(role);
        //}

        return  new Tuple<int, string>(200, $"Role {command.Name} created"); 
        //new ApiResponse(200, $"Role {roleDto.Name} created", roleDto); //fix a strange System.Text.Json exception shown only in Debug_SSB

    }
}


//// Use for Idempotency in Command process
//public class CancelOrderIdentifiedCommandHandler : IdentifiedCommandHandler<CancelOrderCommand, bool>
//{
//    public CancelOrderIdentifiedCommandHandler(
//        IMediator mediator,
//        IRequestManager requestManager,
//        ILogger<IdentifiedCommandHandler<CancelOrderCommand, bool>> logger)
//        : base(mediator, requestManager, logger)
//    {
//    }

//    protected override bool CreateResultForDuplicateRequest()
//    {
//        return true; // Ignore duplicate requests for processing order.
//    }
//}
