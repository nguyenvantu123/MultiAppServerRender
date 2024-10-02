
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

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Tuple<int, string>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateUserCommandHandler( UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Handler which processes the command when
    /// customer executes cancel order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<Tuple<int, string>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {

        var user = new ApplicationUser
        {
            UserName = command.UserName,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PhoneNumber = command.PhoneNumber,
            IsActive = command.ActivateUser,
            EmailConfirmed = command.AutoConfirmEmail
        };

        var checkUserIsExist = await _userManager.FindByNameAsync(command.UserName);
        if (checkUserIsExist != null)
        {
            return new Tuple<int, string>(400, "User Name đã tồn tại");
        }

        checkUserIsExist = await _userManager.FindByEmailAsync(command.Email);

        if (checkUserIsExist != null)
        {
            return new Tuple<int, string>(400, "Email đã tồn tại");
        }

        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var msg = result.GetErrors();

            return new Tuple<int, string>(400, msg);
        }

        var claimsResult = _userManager.AddClaimsAsync(user, new Claim[]{
                        new Claim(Policies.IsUser, string.Empty),
                        new Claim(ClaimTypes.Name, command.UserName),
                        new Claim(ClaimTypes.Email, command.Email),
                        new Claim(ApplicationClaimTypes.EmailVerified, ClaimValues.falseString, ClaimValueTypes.Boolean)
                    }).Result;

        //if (_userManager.Options.SignIn.RequireConfirmedEmail)
        //{
        //    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    string callbackUrl = string.Format("{0}/Account/ConfirmEmail/{1}?token={2}", baseUrl, user.Id, token);

        //    var email = _emailFactory.BuildNewUserConfirmationEmail($"{user.FirstName} {user.LastName}", user.UserName, callbackUrl);
        //    email.ToAddresses.Add(new EmailAddressDto(user.Email, user.Email));

        //    var response = await _emailFactory.QueueEmail(email, EmailType.Confirmation);

        //    if (!response.IsSuccessStatusCode)
        //        _logger.LogError($"New user email failed: {response.Message}");

        //}
        //else
        //{
        //    var email = _emailFactory.BuildNewUserEmail($"{user.FirstName} {user.LastName}", user.UserName, user.Email, parameters.Password);
        //    email.ToAddresses.Add(new EmailAddressDto(user.Email, user.Email));

        //    var response = await _emailFactory.SendEmail(email);

        //    if (!response.IsSuccessStatusCode)
        //        _logger.LogError($"New user email failed: {response.Message}");

        //}

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
