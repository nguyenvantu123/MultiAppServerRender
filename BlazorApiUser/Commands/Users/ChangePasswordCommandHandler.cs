
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

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Tuple<int, string>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Handler which processes the command when
    /// customer executes cancel order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<Tuple<int, string>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {

        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null)
        {
            return new Tuple<int, string>(200, $"User not found!!!");

        }
        var passToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, passToken, command.Password);
        if (!result.Succeeded)
        {
            var msg = result.GetErrors();
            return new Tuple<int, string>(400, msg);
        }
        //else
        //{
        //    _logger.LogWarning(user.UserName + "'s password reset failed; Requested from Admin interface by:" + User.Identity.Name);

        //    var msg = result.GetErrors();
        //    _logger.LogWarning($"Error while resetting password of {user.UserName}: {msg}");
        //    return new ApiResponse((int)HttpStatusCode.BadRequest, msg);
        //}

        return new Tuple<int, string>(200, $"User {user.UserName}  password reset");

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
