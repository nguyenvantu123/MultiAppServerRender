
// Regular CommandHandler
using Amazon.Runtime.Internal;
using Azure.Core;
using BlazorApiUser.Repository;
using BlazorBoilerplate.Constants;
using BlazorIdentity.Users;
using BlazorIdentity.Users.Constants;
using BlazorIdentity.Users.Extensions;
using BlazorIdentity.Users.Models;
using BlazorIdentity.Users.Models.Email;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MultiAppServer.ServiceDefaults;
using System.Net;
using System.Security.Claims;


namespace BlazorApiUser.Commands.Users;

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, Tuple<int, string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    protected readonly IHttpContextAccessor _httpContextAccessor;


    public UpdateUserRolesCommandHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Handler which processes the command when
    /// customer executes cancel order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<Tuple<int, string>> Handle(UpdateUserRolesCommand command, CancellationToken cancellationToken)
    {

        var user = await _userManager.FindByIdAsync(command.UserId);

        if (user == null)
        {
            return new Tuple<int, string>(400, "Not found user!!!");
        }

        if (user.UserName == DefaultUserNames.Administrator)
        {
            return new Tuple<int, string>(400, "Not Allowed");
        }

        var userAccessor = _httpContextAccessor!.HttpContext!.User;
        var userId = new Guid(userAccessor.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

        var roles = await _userManager.GetRolesAsync(user);
        var selectedRoles = command.UserRoles.Where(x => x.Selected).ToList();

        var currentUser = await _userManager.FindByIdAsync(userId.ToString());

        if (currentUser == null)
        {
            return new Tuple<int, string>(401, "Unauthorized!!!");
        }

        if (!await _userManager.IsInRoleAsync(currentUser, DefaultRoleNames.Administrator))
        {
            var tryToAddAdministratorRole = selectedRoles
                .Any(x => x.RoleName == DefaultRoleNames.Administrator);
            var userHasAdministratorRole = roles.Any(x => x == DefaultRoleNames.Administrator);
            if (tryToAddAdministratorRole && !userHasAdministratorRole || !tryToAddAdministratorRole && userHasAdministratorRole)
            {

                //return new ApiResponseDto(400, "Not Allowed to add or delete Administrator Role if you have not this role.");
                return new Tuple<int, string>(400, "Not Allowed to add or delete Administrator Role if you have not this role.");

                //return await Result.FailAsync(_localizer["Not Allowed to add or delete Administrator Role if you have not this role."]);
            }
        }

        var result = await _userManager.RemoveFromRolesAsync(user, roles);
        result = await _userManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));

        return new Tuple<int, string>(200, "Roles Updated");

        //return new ApiResponseDto(200, "Roles Updated");
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
