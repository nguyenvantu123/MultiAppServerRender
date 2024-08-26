
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


namespace BlazorApiUser.Commands.Users;

public class AdminUpdateUserCommandHandler : IRequestHandler<AdminUpdateUserCommand, Tuple<int, string>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminUpdateUserCommandHandler( UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Handler which processes the command when
    /// customer executes cancel order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<Tuple<int, string>> Handle(AdminUpdateUserCommand command, CancellationToken cancellationToken)
    {


        var user = await _userManager.FindByIdAsync(command.Id.ToString());

        if (user == null)
        {
            return new Tuple<int, string>(404, "User not found!!!");
        }

        if (user.UserName.ToLower() != "admin" && command.UserName.ToLower() != "admin")
            user.UserName = command.UserName;

        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.Email = command.Email;


        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var msg = result.GetErrors();

            return new Tuple<int, string>(400, msg);

        }

        return new Tuple<int, string>(200, $"User {command.UserName} created");
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
