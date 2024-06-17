// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using BlazorBoilerplate.Constants;
using BlazorBoilerplate.Infrastructure.AuthorizationDefinitions;
using BlazorBoilerplate.Infrastructure.Server;
using BlazorBoilerplate.Infrastructure.Server.Models;
using BlazorBoilerplate.Infrastructure.Storage.Permissions;
using BlazorBoilerplate.Server.Extensions;
using BlazorBoilerplate.Shared.Dto.Email;
using BlazorBoilerplate.Shared.Localizer;
using BlazorBoilerplate.Shared.Models.Account;
using BlazorWebApi.Users.Models;
using BlazorWebApi.Users.Models.AccountViewModels;
using BlazorWebApi.Users.Models.ManageViewModels;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Localization;
using NSwag.Annotations;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using WebApp.Models;

namespace IdentityServerHost.Quickstart.UI
{
    [OpenApiIgnore]
    [SecurityHeaders]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IAuthenticationHandlerProvider _handlerProvider;
        private readonly IEventService _events;

        private readonly IStringLocalizer<Global> L;

        private readonly ILogger<AccountController> _logger;
        private readonly string baseUrl;

        private readonly IEmailFactory _emailFactory;

        private readonly UrlEncoder _urlEncoder;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IAuthenticationHandlerProvider handlerProvider,
            ILogger<AccountController> logger,
            IEventService events,
            IConfiguration configuration,
            IEmailFactory emailFactory,
            UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _handlerProvider = handlerProvider;
            _events = events;
            _logger = logger;
            baseUrl = configuration[$"{nameof(BlazorBoilerplate)}:ApplicationUrl"];

            _emailFactory = emailFactory;
            _urlEncoder = urlEncoder;
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>

        [HttpPost("BuildLoginViewModel")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ApiResponse> BuildLoginViewModel(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null ||
                            x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase)
                )
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;

            return new ApiResponse((int)HttpStatusCode.OK, L["Operation Successful"], new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                ExternalProviders = providers.ToArray()
            });
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
         // POST: api/Account/Login
        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ApiResponse> Login(LoginInputModel parameters)
        {

            if (!ModelState.IsValid)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["InvalidData"]);
            }

            try
            {
                //await _databaseInitializer.EnsureAdminIdentitiesAsync();

                var result = await _signInManager.PasswordSignInAsync(parameters.Username, parameters.Password, parameters.RememberLogin, true);

                if (result.RequiresTwoFactor)
                {
                    _logger.LogInformation("Two factor authentication required for user {0}", parameters.Username);

                    return new ApiResponse((int)HttpStatusCode.OK, "Two factor authentication required")
                    {
                        Result = new LoginResponseModel()
                        {
                            RequiresTwoFactor = true
                        }
                    };
                }

                // If lock out activated and the max. amounts of attempts is reached.
                if (result.IsLockedOut)
                {
                    _logger.LogInformation("User Locked out: {0}", parameters.Username);
                    return new ApiResponse((int)HttpStatusCode.Unauthorized, L["LockedUser"]);
                }

                // If your email is not confirmed but you require it in the settings for login.
                if (result.IsNotAllowed)
                {
                    _logger.LogInformation("User {0} not allowed to log in, because email is not confirmed", parameters.Username);
                    return new ApiResponse((int)HttpStatusCode.Unauthorized, L["EmailNotConfirmed"]);
                }

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(parameters.Username);
                    _logger.LogInformation("Logged In user {0}", parameters.Username);

                    //TODO parameters.IsValidReturnUrl is set true above 
                    //if (!parameters.IsValidReturnUrl)
                    //    // user might have clicked on a malicious link - should be logged
                    //    throw new Exception("invalid return URL");

                    return new ApiResponse((int)HttpStatusCode.OK);
                }

                _logger.LogInformation("Invalid Password for user {0}", parameters.Username);
                return new ApiResponse((int)HttpStatusCode.Unauthorized, L["LoginFailed"]);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login Failed: {ex.GetBaseException().Message}");
                return new ApiResponse((int)HttpStatusCode.InternalServerError, L["LoginFailed"]);
            }
        }

        // POST: api/Account/LoginWithRecoveryCode
        [HttpPost("LoginWithRecoveryCode")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ApiResponse> LoginWithRecoveryCode(LoginWithRecoveryCodeInputModel parameters)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["InvalidData"]);
            }

            try
            {
                // Ensure the user has gone through the username & password screen first
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

                if (user == null)
                {
                    return new ApiResponse((int)HttpStatusCode.NotFound, "Unable to load two-factor authentication user.");
                }

                var recoveryCode = parameters.RecoveryCode.Replace(" ", string.Empty);

                var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

                // If lock out activated and the max. amounts of attempts is reached.
                if (result.IsLockedOut)
                {
                    _logger.LogInformation("User Locked out: {0}", user.UserName);
                    return new ApiResponse((int)HttpStatusCode.Unauthorized, L["LockedUser"]);
                }

                // If your email is not confirmed but you require it in the settings for login.
                if (result.IsNotAllowed)
                {
                    _logger.LogInformation("User {0} not allowed to log in, because email is not confirmed", user.UserName);
                    return new ApiResponse((int)HttpStatusCode.Unauthorized, L["EmailNotConfirmed"]);
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("User '{0}' logged in with a recovery code", user.UserName);

                    return new ApiResponse((int)HttpStatusCode.OK);
                }

                _logger.LogInformation("Invalid recovery code for user {0}", user.UserName);
                return new ApiResponse((int)HttpStatusCode.Unauthorized, L["LoginFailed"]);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login Failed: {ex.GetBaseException().Message}");
                return new ApiResponse((int)HttpStatusCode.InternalServerError, L["LoginFailed"]);
            }
        }

        // POST: api/Account/LoginWith2fa
        [HttpPost("LoginWith2fa")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ApiResponse> LoginWith2fa(LoginWith2faInputModel parameters)
        {

            if (!ModelState.IsValid)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["InvalidData"]);
            }

            try
            {
                // Ensure the user has gone through the username & password screen first
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

                if (user == null)
                {
                    return new ApiResponse((int)HttpStatusCode.NotFound, "Unable to load two-factor authentication user.");
                }

                var authenticatorCode = parameters.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

                var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, parameters.RememberMe, parameters.RememberMachine);

                // If lock out activated and the max. amounts of attempts is reached.
                if (result.IsLockedOut)
                {
                    _logger.LogInformation("User Locked out: {0}", user.UserName);
                    return new ApiResponse((int)HttpStatusCode.Unauthorized, L["LockedUser"]);
                }

                // If your email is not confirmed but you require it in the settings for login.
                if (result.IsNotAllowed)
                {
                    _logger.LogInformation("User {0} not allowed to log in, because email is not confirmed", user.UserName);
                    return new ApiResponse((int)HttpStatusCode.Unauthorized, L["EmailNotConfirmed"]);
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("User '{0}' logged in with a authenticator code", user.UserName);

                    return new ApiResponse((int)HttpStatusCode.OK);
                }

                _logger.LogInformation("Invalid authenticator code for user {0}", user.UserName);
                return new ApiResponse((int)HttpStatusCode.Unauthorized, L["LoginFailed"]);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login Failed: {ex.GetBaseException().Message}");
                return new ApiResponse((int)HttpStatusCode.InternalServerError, L["LoginFailed"]);
            }
        }



        /// <summary>
        /// Show logout page
        /// </summary>
        // POST: api/Account/Logout
        [HttpPost("Logout")]
        [Authorize]
        public async Task<ApiResponse> Logout()
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();
            }

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                LogoutId = null
            };


            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return new ApiResponse((int)HttpStatusCode.OK, "Logout Successful"); ;
        }



        // POST: api/Account/Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ApiResponse> Register(RegisterViewModel parameters)
        {

            if (!ModelState.IsValid)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["InvalidData"]);
            }

            await RegisterNewUserAsync(parameters.UserName, parameters.Email, parameters.Password, _userManager.Options.SignIn.RequireConfirmedEmail);

            if (_userManager.Options.SignIn.RequireConfirmedEmail)
                return new ApiResponse((int)HttpStatusCode.OK, L["Operation Successful"]);
            else
            {
                return await Login(new LoginInputModel
                {
                    Username = parameters.UserName,
                    Password = parameters.Password
                });
            }
        }

        private async Task<ApplicationUser> RegisterNewUserAsync(string userName, string email, string password, bool requireConfirmEmail)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email
            };

            return await RegisterNewUserAsync(user, password, requireConfirmEmail);
        }

        private async Task<ApplicationUser> RegisterNewUserAsync(ApplicationUser user, string password, bool requireConfirmEmail)
        {
            var result = password == null ?
                await _userManager.CreateAsync(user) :
                await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new DomainException(result.GetErrors());

            await _userManager.AddClaimsAsync(user, new Claim[]{
                    new Claim(Policies.IsUser, string.Empty),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ApplicationClaimTypes.EmailVerified, ClaimValues.falseString, ClaimValueTypes.Boolean)
                });

            _logger.LogInformation("New user registered: {0}", user);

            EmailMessageDto emailMessage;

            if (requireConfirmEmail)
            {
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = $"{baseUrl}/Account/ConfirmEmail/{user.Id}?token={token}";

                emailMessage = _emailFactory.BuildNewUserConfirmationEmail($"{user.FirstName} {user.LastName}", user.UserName, callbackUrl);
            }
            else
            {
                emailMessage = _emailFactory.BuildNewUserEmail($"{user.FirstName} {user.LastName}", user.UserName, user.Email, password);
            }

            emailMessage.ToAddresses.Add(new EmailAddressDto(user.Email, user.Email));

            var response = requireConfirmEmail ? await _emailFactory.QueueEmail(emailMessage, EmailType.Confirmation) : await _emailFactory.SendEmail(emailMessage);

            if (response.IsSuccessStatusCode)
                _logger.LogInformation($"New user email sent to {user.Email}");
            else
                _logger.LogError("New user email failed: Body: {0}, Error: {1}", emailMessage.Body, response.Message);

            return user;
        }


        // POST: api/Account/ConfirmEmail
        [HttpPost("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<ApiResponse> ConfirmEmail(ConfirmEmailViewModel parameters)

        {

            if (!ModelState.IsValid)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["InvalidData"]);
            }

            if (parameters.UserId == null || parameters.Token == null)
            {
                return new ApiResponse((int)HttpStatusCode.NotFound, L["The user doesn't exist"]);
            }

            var user = await _userManager.FindByIdAsync(parameters.UserId);

            if (user == null)
            {
                _logger.LogInformation(L["The user {0} doesn't exist", parameters.UserId]);
                return new ApiResponse((int)HttpStatusCode.NotFound, L["The user doesn't exist"]);
            }

            if (!user.EmailConfirmed)
            {
                var token = parameters.Token;
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (!result.Succeeded)
                {
                    var msg = result.GetErrors();
                    _logger.LogWarning("User Email Confirmation Failed: {0}", msg);
                    return new ApiResponse((int)HttpStatusCode.BadRequest, msg);
                }

                await _userManager.RemoveClaimAsync(user, new Claim(ApplicationClaimTypes.EmailVerified, ClaimValues.falseString, ClaimValueTypes.Boolean));
                await _userManager.AddClaimAsync(user, new Claim(ApplicationClaimTypes.EmailVerified, ClaimValues.trueString, ClaimValueTypes.Boolean));
            }

            return new ApiResponse((int)HttpStatusCode.OK, L["EmailVerificationSuccessful"]);
        }

        // POST: api/Account/ForgotPassword
        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<ApiResponse> ForgotPassword(ForgotPasswordViewModel parameters)
        {

            if (!ModelState.IsValid)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["InvalidData"]);
            }

            var user = await _userManager.FindByEmailAsync(parameters.Email);

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogInformation("Forgot Password with non-existent email / user: {0}", parameters.Email);
                // Don't reveal that the user does not exist or is not confirmed
                return new ApiResponse((int)HttpStatusCode.OK, L["Operation Successful"]);
            }

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string callbackUrl = string.Format("{0}/Account/ResetPassword/{1}?token={2}", baseUrl, user.Id, token); //token must be a query string parameter as it is very long

            var email = _emailFactory.BuildForgotPasswordEmail(user.UserName, callbackUrl, token);
            email.ToAddresses.Add(new EmailAddressDto(user.Email, user.Email));

            var response = await _emailFactory.QueueEmail(email, EmailType.Password);

            if (response.IsSuccessStatusCode)
                _logger.LogInformation($"Reset Password Successful Email Sent: {user.Email}");
            else
                _logger.LogError($"Reset Password Successful Email Sent: {user.Email}");

            return response;
        }

        //api/Account/ResetPassword
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<ApiResponse> ResetPassword(ResetPasswordViewModel parameters)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["InvalidData"]);
            }

            var user = await _userManager.FindByIdAsync(parameters.UserId);
            if (user == null)
            {
                _logger.LogInformation(L["The user {0} doesn't exist", parameters.UserId]);
                return new ApiResponse((int)HttpStatusCode.NotFound, L["The user doesn't exist"]);
            }

            var result = await _userManager.ResetPasswordAsync(user, parameters.Token, parameters.Password);

            if (result.Succeeded)
            {
                var email = _emailFactory.BuildPasswordResetEmail(user.UserName);
                email.ToAddresses.Add(new EmailAddressDto(user.Email, user.Email));

                var response = await _emailFactory.QueueEmail(email, EmailType.Password);

                if (response.IsSuccessStatusCode)
                    _logger.LogInformation($"Reset Password Successful Email to {user.Email}");
                else
                    _logger.LogError($"Fail to send Reset Password Email to {user.Email}");

                return response;
            }
            else
            {
                var msg = result.GetErrors();
                _logger.LogWarning("Error while resetting the password: {0}", msg);
                return new ApiResponse((int)HttpStatusCode.BadRequest, msg);
            }
        }

        //api/Account/UpdatePassword
        [HttpPost("UpdatePassword")]
        public async Task<ApiResponse> UpdatePassword(UpdatePasswordViewModel parameters)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["InvalidData"]);
            }

            var user = await _userManager.FindByIdAsync(User.GetSubjectById());
            if (user == null)
            {
                _logger.LogInformation(L["The user {0} doesn't exist", User.GetDisplayByName()]);
                return new ApiResponse((int)HttpStatusCode.NotFound, L["The user doesn't exist"]);
            }

            var result = await _userManager.ChangePasswordAsync(user, parameters.CurrentPassword, parameters.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return new ApiResponse((int)HttpStatusCode.OK, L["Operation Successful"]);
            }
            else
            {
                var msg = result.GetErrors();
                _logger.LogWarning($"Error while updating the password of {user.UserName}: {msg}");
                return new ApiResponse((int)HttpStatusCode.BadRequest, msg);
            }
        }

        [HttpPost("EnableAuthenticator")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ApiResponse> EnableAuthenticator(AuthenticatorVerificationCodeViewModel parameters)
        {

            if (!ModelState.IsValid)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["InvalidData"]);
            }

            var user = await _userManager.FindByIdAsync(User.GetSubjectById());
            if (user == null)
            {
                _logger.LogInformation(L["The user {0} doesn't exist", User.GetDisplayByName()]);
                return new ApiResponse((int)HttpStatusCode.NotFound, L["The user doesn't exist"]);
            }

            var verificationCode = parameters.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (is2faTokenValid)
            {
                var result = await _userManager.SetTwoFactorEnabledAsync(user, true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User '{0}' has enabled 2FA with an authenticator app.", user.UserName);

                    var userViewModel = await BuildUserViewModel(User);

                    if (await _userManager.CountRecoveryCodesAsync(user) == 0)
                    {
                        userViewModel.RecoveryCodes = (await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10)).ToArray();
                    }

                    return new ApiResponse((int)HttpStatusCode.OK, L["Operation Successful"], userViewModel);
                }
                else
                    return new ApiResponse((int)HttpStatusCode.InternalServerError, "Error while enabling 2FA");
            }
            else
            {
                _logger.LogWarning($"Verification code of {user.UserName} is invalid.");
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["VerificationCodeInvalid"]);
            }
        }

        private async Task<UserViewModel> BuildUserViewModel(ClaimsPrincipal authenticatedUser)
        {
            var user = await _userManager.GetUserAsync(authenticatedUser);

            if (user != null)
            {
                var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);

                var userViewModel = new UserViewModel
                {
                    IsAuthenticated = authenticatedUser.Identity.IsAuthenticated,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserId = user.Id,
                    HasPassword = await _userManager.HasPasswordAsync(user),
                    PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                    TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                    HasAuthenticator = !string.IsNullOrEmpty(unformattedKey),
                    BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                    CountRecoveryCodes = await _userManager.CountRecoveryCodesAsync(user),

                    Logins = (await _userManager.GetLoginsAsync(user)).Select(i => new KeyValuePair<string, string>(i.LoginProvider, i.ProviderKey)).ToList(),

                    ExposedClaims = authenticatedUser.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value)).ToList(),
                    Roles = ((ClaimsIdentity)authenticatedUser.Identity).Claims
                            .Where(c => c.Type == "role")
                            .Select(c => c.Value).ToList()
                };

                if (!userViewModel.TwoFactorEnabled)
                {
                    if (!userViewModel.HasAuthenticator)
                    {
                        await _userManager.ResetAuthenticatorKeyAsync(user);
                        unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                    }

                    userViewModel.SharedKey = FormatKey(unformattedKey);
                    userViewModel.AuthenticatorUri = string.Format(
                        "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                        _urlEncoder.Encode("BlazorBoilerplate"),
                        _urlEncoder.Encode(user.Email),
                        unformattedKey);
                }

                return userViewModel;
            }
            else
            {
                return new UserViewModel();
            }
        }
        private static string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;

            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey[currentPosition..]);
            }

            return result.ToString().ToUpperInvariant();
        }

        [HttpPost("DisableAuthenticator")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ApiResponse> DisableAuthenticator()
        {
            var user = await _userManager.FindByIdAsync(User.GetSubjectById());
            if (user == null)
            {
                _logger.LogInformation(L["The user {0} doesn't exist", User.GetDisplayByName()]);
                return new ApiResponse((int)HttpStatusCode.NotFound, L["The user doesn't exist"]);
            }

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);

            if (result.Succeeded)
            {
                result = await _userManager.ResetAuthenticatorKeyAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

                    await _signInManager.RefreshSignInAsync(user);
                }
                else
                    return new ApiResponse((int)HttpStatusCode.BadRequest, "Error while disabling authenticator");
            }
            else
                return new ApiResponse((int)HttpStatusCode.BadRequest, "Error while disabling 2fa");

            return new ApiResponse((int)HttpStatusCode.OK, L["Operation Successful"], await BuildUserViewModel(User));
        }

        [HttpPost("ForgetTwoFactorClient")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ApiResponse> ForgetTwoFactorClient()
        {
            var user = await _userManager.FindByIdAsync(User.GetSubjectById());
            if (user == null)
            {
                _logger.LogInformation(L["The user {0} doesn't exist", User.GetDisplayByName()]);
                return new ApiResponse((int)HttpStatusCode.NotFound, L["The user doesn't exist"]);
            }

            await _signInManager.ForgetTwoFactorClientAsync();

            return new ApiResponse((int)HttpStatusCode.OK, L["Operation Successful"], await BuildUserViewModel(User));
        }

        [HttpPost("Enable2fa")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ApiResponse> Enable2fa()
        {
            var user = await _userManager.FindByIdAsync(User.GetSubjectById());
            if (user == null)
            {
                _logger.LogInformation(L["The user {0} doesn't exist", User.GetDisplayByName()]);
                return new ApiResponse((int)HttpStatusCode.NotFound, L["The user doesn't exist"]);
            }

            var result = await _userManager.SetTwoFactorEnabledAsync(user, true);

            if (result.Succeeded)
            {
                return new ApiResponse((int)HttpStatusCode.OK, "Enabling 2fa Successful", await BuildUserViewModel(User));
            }
            else
                return new ApiResponse((int)HttpStatusCode.BadRequest, "Error while enabling 2fa");
        }

        [HttpPost("Disable2fa")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ApiResponse> Disable2fa()
        {
            var user = await _userManager.FindByIdAsync(User.GetSubjectById());
            if (user == null)
            {
                _logger.LogInformation(L["The user {0} doesn't exist", User.GetDisplayByName()]);
                return new ApiResponse((int)HttpStatusCode.NotFound, L["The user doesn't exist"]);
            }

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);

            if (result.Succeeded)
            {
                return new ApiResponse((int)HttpStatusCode.OK, "Disabling 2fa Successful", await BuildUserViewModel(User));
            }
            else
                return new ApiResponse((int)HttpStatusCode.BadRequest, "Error while disabling 2fa");
        }

        [HttpGet("UserViewModel")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ApiResponse> UserViewModel()
        {
            var userViewModel = await BuildUserViewModel(User);
            return new ApiResponse((int)HttpStatusCode.OK, L["Operation Successful"], userViewModel);
        }

        [HttpPost("UpdateUser")]
        [Authorize]
        public async Task<ApiResponse> UpdateUser(UserViewModel userViewModel)
        {

            var user = await _userManager.FindByEmailAsync(userViewModel.Email);

            if (user == null)
            {
                _logger.LogInformation(L["The user {0} doesn't exist", userViewModel.Email]);
                return new ApiResponse((int)HttpStatusCode.NotFound, L["The user doesn't exist"]);
            }

            user.FirstName = userViewModel.FirstName;
            user.LastName = userViewModel.LastName;
            user.Email = userViewModel.Email;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var msg = result.GetErrors();
                _logger.LogWarning("User Update Failed: {0}", msg);
                return new ApiResponse((int)HttpStatusCode.BadRequest, msg);
            }

            return new ApiResponse((int)HttpStatusCode.OK, L["Operation Successful"]);
        }

        ///----------Admin User Management Interface Methods
        // POST: api/Account/Create
        [HttpPost("Create")]
        [Authorize(Permissions.User.Create)]
        public async Task<ApiResponse> Create(RegisterViewModel parameters)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["InvalidData"]);
            }

            var user = new ApplicationUser
            {
                UserName = parameters.UserName,
                Email = parameters.Email
            };

            var result = await _userManager.CreateAsync(user, parameters.Password);

            if (!result.Succeeded)
            {
                var msg = result.GetErrors();
                _logger.LogWarning($"Error while creating {user.UserName}: {msg}");
                return new ApiResponse((int)HttpStatusCode.NotFound, msg);
            }
            else
            {
                var claimsResult = _userManager.AddClaimsAsync(user, new Claim[]{
                        new Claim(Policies.IsUser, string.Empty),
                        new Claim(ClaimTypes.Name, parameters.UserName),
                        new Claim(ClaimTypes.Email, parameters.Email),
                        new Claim(ApplicationClaimTypes.EmailVerified, ClaimValues.falseString, ClaimValueTypes.Boolean)
                    }).Result;
            }

            if (_userManager.Options.SignIn.RequireConfirmedEmail)
            {
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string callbackUrl = string.Format("{0}/Account/ConfirmEmail/{1}?token={2}", baseUrl, user.Id, token);

                var email = _emailFactory.BuildNewUserConfirmationEmail($"{user.FirstName} {user.LastName}", user.UserName, callbackUrl);
                email.ToAddresses.Add(new EmailAddressDto(user.Email, user.Email));

                _logger.LogInformation("New user created: {0}", user);
                var response = await _emailFactory.QueueEmail(email, EmailType.Confirmation);

                if (!response.IsSuccessStatusCode)
                    _logger.LogError($"New user email failed: {response.Message}");

                return new ApiResponse((int)HttpStatusCode.OK, "Create User Success");
            }
            else
            {
                var email = _emailFactory.BuildNewUserEmail($"{user.FirstName} {user.LastName}", user.UserName, user.Email, parameters.Password);
                email.ToAddresses.Add(new EmailAddressDto(user.Email, user.Email));

                _logger.LogInformation("New user created: {0}", user);

                var response = await _emailFactory.SendEmail(email);

                if (!response.IsSuccessStatusCode)
                    _logger.LogError($"New user email failed: {response.Message}");

                var userViewModel = new UserViewModel
                {
                    UserId = user.Id,
                    IsAuthenticated = false,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                return new ApiResponse((int)HttpStatusCode.OK, L["User {0} created", userViewModel.UserName], userViewModel);
            }
        }

        // DELETE: api/Account/5
        [HttpDelete("{id}")]
        [Authorize(Permissions.User.Delete)]
        public async Task<ApiResponse> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning(L["The user {0} doesn't exist", id]);
                return new ApiResponse((int)HttpStatusCode.NotFound, L["The user doesn't exist"]);
            }
            if (user.UserName.ToLower() != "admin")
            {
                //TODO it could generate time-out
                //await _userProfileStore.DeleteAllApiLogsForUser(user.Id);

                await _userManager.DeleteAsync(user);
                return new ApiResponse((int)HttpStatusCode.OK, "User Deletion Successful");
            }
            else
                return new ApiResponse((int)HttpStatusCode.Forbidden, L["User {0} cannot be edited", user.UserName]);
        }

        [HttpGet("GetUser")]
        public ApiResponse GetUser()
        {
            UserViewModel userViewModel = User != null && User.Identity.IsAuthenticated
              ? new UserViewModel { UserName = User.Identity.Name, IsAuthenticated = true }
              : new()
              {
                  IsAuthenticated = false,
                  Roles = new List<string>()
              };

            return new ApiResponse((int)HttpStatusCode.OK, L["Operation Successful"], userViewModel);
        }

        [HttpPost("AdminUpdateUser")]
        [Authorize(Permissions.User.Update)]
        public async Task<ApiResponse> AdminUpdateUser([FromBody] UserViewModel userViewModel)
        {

            if (!ModelState.IsValid)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["InvalidData"]);
            }

            var user = await _userManager.FindByIdAsync(userViewModel.UserId.ToString());

            if (user.UserName.ToLower() != "admin" && userViewModel.UserName.ToLower() != "admin")
                user.UserName = userViewModel.UserName;

            user.FirstName = userViewModel.FirstName;
            user.LastName = userViewModel.LastName;
            user.Email = userViewModel.Email;

            try
            {
                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Updating user exception: {ex.GetBaseException().Message}");
                return new ApiResponse((int)HttpStatusCode.InternalServerError, L["Operation Failed"]);
            }

            if (userViewModel.Roles != null)
            {
                try
                {
                    var rolesToAdd = new List<string>();
                    var currentUserRoles = (List<string>)await _userManager.GetRolesAsync(user);

                    foreach (var newUserRole in userViewModel.Roles)
                    {
                        if (!currentUserRoles.Contains(newUserRole))
                            rolesToAdd.Add(newUserRole);
                    }

                    if (rolesToAdd.Count > 0)
                    {
                        await _userManager.AddToRolesAsync(user, rolesToAdd);

                        //HACK to switch to claims auth
                        foreach (var role in rolesToAdd)
                            await _userManager.AddClaimAsync(user, new Claim($"Is{role}", ClaimValues.trueString));
                    }

                    var rolesToRemove = currentUserRoles.Where(role => !userViewModel.Roles.Contains(role)).ToList();

                    if (rolesToRemove.Count > 0)
                    {
                        if (user.UserName.ToLower() == "admin")
                            rolesToRemove.Remove("admin");

                        await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                        //HACK to switch to claims auth
                        foreach (var role in rolesToRemove)
                            await _userManager.RemoveClaimAsync(user, new Claim($"Is{role}", ClaimValues.trueString));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Updating Roles exception: {ex.GetBaseException().Message}");
                    return new ApiResponse((int)HttpStatusCode.InternalServerError, L["Operation Failed"]);
                }
            }

            return new ApiResponse((int)HttpStatusCode.OK, L["Operation Successful"]);
        }

        [HttpPost("AdminUserPasswordReset")]
        [Authorize(Permissions.User.Update)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ApiResponse> AdminResetUserPasswordAsync(ChangePasswordViewModel changePasswordViewModel)
        {

            if (!ModelState.IsValid)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, L["InvalidData"]);
            }

            var user = await _userManager.FindByIdAsync(changePasswordViewModel.UserId);
            if (user == null)
            {
                _logger.LogWarning(L["The user {0} doesn't exist", changePasswordViewModel.UserId]);
                return new ApiResponse((int)HttpStatusCode.NotFound, L["The user doesn't exist"]);
            }
            var passToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, passToken, changePasswordViewModel.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation(user.UserName + "'s password reset; Requested from Admin interface by:" + User.Identity.Name);
                return new ApiResponse((int)HttpStatusCode.NoContent, user.UserName + " password reset");
            }
            else
            {
                _logger.LogWarning(user.UserName + "'s password reset failed; Requested from Admin interface by:" + User.Identity.Name);

                var msg = result.GetErrors();
                _logger.LogWarning($"Error while resetting password of {user.UserName}: {msg}");
                return new ApiResponse((int)HttpStatusCode.BadRequest, msg);
            }
        }
    }
}