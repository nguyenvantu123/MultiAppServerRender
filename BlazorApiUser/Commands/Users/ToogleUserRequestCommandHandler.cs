
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

public class ToogleUserRequestCommandHandler : IRequestHandler<ToogleUserRequestCommand, Tuple<int, string>>
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    protected readonly IHttpContextAccessor _httpContextAccessor;


    public ToogleUserRequestCommandHandler(IUserRepository userRepository, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Handler which processes the command when
    /// customer executes cancel order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<Tuple<int, string>> Handle(ToogleUserRequestCommand command, CancellationToken cancellationToken)
    {

        var userById = await _userManager.FindByIdAsync(command.UserId);

        if (userById == null)
        {
            return new Tuple<int, string>(404, "User not found!!!");
        }

        userById.IsActive = command.ActivateUser;

        await _userManager.UpdateAsync(userById);

        return new Tuple<int, string>(200, "Success!!!");
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
