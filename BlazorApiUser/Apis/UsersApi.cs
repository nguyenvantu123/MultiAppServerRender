

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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shared.Models;
using BlazorApiUser.Models;
using BlazorApiUser.Constants;
using BlazorApiUser.Repositories;
using BlazorApiUser.Extensions;
using Minio;
using Minio.DataModel.Args;
using Aspire.Minio.Client;

public static class UsersApi
{
    public static RouteGroupBuilder MapUsersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/admin").HasApiVersion(1.0);

        api.MapGet("/users", GetUsers);

        api.MapPost("/users", Create);

        api.MapPut("/users/{id}", AdminUpdateUser);

        api.MapDelete("/users/{id}", AdminDelete);

        api.MapPost("/users/reset-password", ResetPasswordUser);

        api.MapGet("/users/get-permission-by-user", GetPermissionByUser);

        api.MapGet("/users/{id}", GetUserById);

        api.MapGet("/users/user-roles/{id}", GetRoleByUserId);

        api.MapPut("/users/update-user-roles/{id}", UpdateUserRoles);

        api.MapPut("/users/toggle-user-status", ToggleUserStatus);

        api.MapGet("/users/user-view-model", UserViewModel);

        api.MapGet("/users/user-profile", UserProfile);

        api.MapPost("/users/update-user-profile", UploadProfile).DisableAntiforgery();


        return api;
    }

    [Authorize(Roles = Permissions.User.Read)]
    public static async Task<ApiResponseDto<List<UserDataViewModel>>> GetUsers([AsParameters] GetListUserQuery getListUserQuery, [AsParameters] UserServices userServices)
    {
        var userLst = await userServices.Mediator.Send(getListUserQuery);

        return new ApiResponseDto<List<UserDataViewModel>>(200, $"{userLst.Item1} users fetched", userLst.Item2, userLst.Item1);
    }

    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponseDto> ToggleUserStatus(ToogleUserRequestCommand toggleUserStatusCommand, [AsParameters] UserServices userServices)
    {

        var userById = await userServices.UserManager.FindByIdAsync(toggleUserStatusCommand.UserId);

        if (userById == null)
        {
            return new ApiResponseDto(404, "User not found!!!");
        }

        userById.IsActive = toggleUserStatusCommand.ActivateUser;

        await userServices.UserManager.UpdateAsync(userById);

        return new ApiResponseDto(200, "Success!!!");
    }

    [Authorize(Roles = Permissions.User.Create)]
    public static async Task<ApiResponseDto> Create(CreateUserCommand command, [AsParameters] UserServices userServices)
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

        var checkUserIsExist = await userServices.UserManager.FindByNameAsync(command.UserName);
        if (checkUserIsExist != null)
        {
            return new ApiResponseDto(400, "User Name is exist");
        }

        checkUserIsExist = await userServices.UserManager.FindByEmailAsync(command.Email);

        if (checkUserIsExist != null)
        {
            return new ApiResponseDto(400, "Email is exist");
        }

        var result = await userServices.UserManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var msg = result.GetErrors();

            return new ApiResponseDto(400, msg);
        }

        var claimsResult = userServices.UserManager.AddClaimsAsync(user, new Claim[]{
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

        return new ApiResponseDto(200, $"User {command.UserName} created");
    }

    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponseDto> AdminUpdateUser(string id, AdminUpdateUserCommand command, [AsParameters] UserServices userServices)
    {
        //var sendCommand = await userServices.Mediator.Send(command);

        var user = await userServices.UserManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ApiResponseDto(400, "User not found!!!", null);
        }

        if (user.UserName == "admin")
        {
            return new ApiResponseDto(400, "Can't update admin user!!!", null);
        }

        //user.UserName = command.UserName;

        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.PhoneNumber = command.PhoneNumber;

        var result = await userServices.UserManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return new ApiResponseDto(400, string.Join(";", result.Errors), null);
        }

        return new ApiResponseDto(200, $"{command.UserName} update successfully!!!", null);
    }

    [Authorize(Roles = Permissions.User.Delete)]
    public static async Task<ApiResponseDto> AdminDelete(string id, [AsParameters] UserServices userServices)
    {

        var user = await userServices.UserManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ApiResponseDto(400, "User not found!!!", null);
        }

        if (user.UserName == DefaultUserNames.Administrator)
        {
            return new ApiResponseDto(400, "Can't delete admin user!!!", null);
        }

        user.IsDeleted = true;

        var result = await userServices.UserManager.UpdateAsync(user);

        await userServices.ApplicationDbContext.SaveChangesAsync();

        if (!result.Succeeded)
        {
            return new ApiResponseDto(400, string.Join(";", result.Errors), null);
        }

        return new ApiResponseDto(200, $"{user.UserName} delete successfully!!!", null);
    }

    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponseDto> ResetPasswordUser(ChangePasswordCommand changePasswordCommand, [AsParameters] UserServices userServices)
    {

        var user = await userServices.UserManager.FindByIdAsync(changePasswordCommand.UserId);
        if (user == null)
        {
            return new ApiResponseDto(200, $"User not found!!!");

        }
        var passToken = await userServices.UserManager.GeneratePasswordResetTokenAsync(user);
        //var result = await userServices.UserManager.ResetPasswordAsync(user, passToken);
        //else
        //{
        //    _logger.LogWarning(user.UserName + "'s password reset failed; Requested from Admin interface by:" + User.Identity.Name);

        //    var msg = result.GetErrors();
        //    _logger.LogWarning($"Error while resetting password of {user.UserName}: {msg}");
        //    return new ApiResponse((int)HttpStatusCode.BadRequest, msg);
        //}

        return new ApiResponseDto(200, $"User {user.UserName}  password reset");
    }

    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponseDto<List<string>>> GetPermissionByUser([AsParameters] UserServices userServices, ClaimsPrincipal userAuth)
    {

        var user = await userServices.UserManager.GetUserAsync(userAuth);

        if (user != null)
        {
            var claims = await userServices.UserManager.GetClaimsAsync(user);

            if (claims != null)
            {
                return new ApiResponseDto<List<string>>(statusCode: 200, message: "", result: claims.ToList().Select(x => x.Value).ToList());

            }
            return new ApiResponseDto<List<string>>(statusCode: 200, message: "", result: new List<string>());
        }

        return new ApiResponseDto<List<string>>(statusCode: 200, message: "", result: new List<string>());
    }

    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponseDto<UserDataViewModel>> GetUserById(string id, [AsParameters] UserServices userServices, IMapper autoMapper)
    {

        var user = await userServices.UserManager.FindByIdAsync(id);

        if (user == null)
        {
            return new ApiResponseDto<UserDataViewModel>(404, "User Not Found", null);
        }

        var result = autoMapper.Map<UserDataViewModel>(user);

        return new ApiResponseDto<UserDataViewModel>(200, "Success", result);
    }
    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponseDto<UserRolesResponse>> GetRoleByUserId(string id, [AsParameters] UserServices userServices, IMapper autoMapper)
    {
        // write unite test for api

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
    [Authorize(Roles = Permissions.User.Update)]
    public static async Task<ApiResponseDto> UpdateUserRoles(string id, UpdateUserRolesCommand updateUserRolesCommand, [AsParameters] UserServices userServices)
    {
        var user = await userServices.UserManager.FindByIdAsync(id);

        if (user == null)
        {
            return new ApiResponseDto(400, "Not found user!!!");
        }

        if (user.UserName == DefaultUserNames.Administrator)
        {
            return new ApiResponseDto(400, "Not Allowed");
        }

        var userAccessor = userServices.HttpContextAccessor!.HttpContext!.User;
        var userId = new Guid(userAccessor.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

        var roles = await userServices.UserManager.GetRolesAsync(user);
        var selectedRoles = updateUserRolesCommand.UserRoles.Where(x => x.Selected).ToList();

        var currentUser = await userServices.UserManager.FindByIdAsync(userId.ToString());

        if (currentUser == null)
        {
            return new ApiResponseDto(401, "Unauthorized!!!");
        }

        if (!await userServices.UserManager.IsInRoleAsync(currentUser, DefaultRoleNames.Administrator))
        {
            var tryToAddAdministratorRole = selectedRoles
                .Any(x => x.RoleName == DefaultRoleNames.Administrator);
            var userHasAdministratorRole = roles.Any(x => x == DefaultRoleNames.Administrator);
            if (tryToAddAdministratorRole && !userHasAdministratorRole || !tryToAddAdministratorRole && userHasAdministratorRole)
            {

                return new ApiResponseDto(400, "Not Allowed to add or delete Administrator Role if you have not this role.");

            }
        }

        var result = await userServices.UserManager.RemoveFromRolesAsync(user, roles);
        result = await userServices.UserManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));

        return new ApiResponseDto(200, "Roles Updated");
    }

    [Authorize]
    public static async Task<ApiResponseDto<UserDataViewModel>> UserViewModel([AsParameters] UserServices userServices, ClaimsPrincipal userAuth, IMapper autoMapper)
    {
        var user = await userServices.UserManager.FindByIdAsync(userAuth!.FindFirstValue("sub")!);

        if (user == null)
        {
            return new ApiResponseDto<UserDataViewModel>(404, "User Not Found", null);
        }

        var result = autoMapper.Map<UserDataViewModel>(user);

        result.Roles = await userServices.UserManager.GetRolesAsync(user);

        return new ApiResponseDto<UserDataViewModel>(200, "Success", result);
    }


    [Authorize]
    public static async Task<ApiResponseDto<UserProfileViewModel>> UserProfile([AsParameters] UserServices userServices, ClaimsPrincipal userAuth, IMapper autoMapper, RedisUserRepository redisUserRepository)
    {

        //ApplicationUser? user = await userServices.UserManager.FindByIdAsync(userId: userAuth.FindFirstValue("sub"));

        //if (user == null)
        //{
        //    return new ApiResponseDto<UserProfileViewModel>(404, "User Not Found", null);
        //}

        UserProfileViewModel dataCache = await redisUserRepository.GetUserProfileAsync(Guid.Parse(userAuth!.FindFirstValue("sub")!));

        if (dataCache == null)
        {
            UserProfile? userProfile = await userServices.ApplicationDbContext.UserProfiles.Where(x => x.UserId.ToString() == userAuth!.FindFirstValue("sub")!).FirstOrDefaultAsync();

            ApplicationUser applicationUser = await userServices.ApplicationDbContext.Users.Where(x => x.Id.ToString() == userAuth!.FindFirstValue("sub")!).FirstOrDefaultAsync();

            if (userProfile == null)
            {
                userProfile = new UserProfile
                {
                    IsDarkMode = false,
                    IsNavOpen = true,
                    LastPageVisited = "/dashboard",
                    UserId = Guid.Parse(userAuth!.FindFirstValue("sub")!),
                    Id = Guid.NewGuid(),
                    FirstName = applicationUser!.FirstName,
                    LastName = applicationUser!.LastName,
                    PhoneNumber = applicationUser!.PhoneNumber,
                    Email = applicationUser!.Email,
                    AvatarUrl = applicationUser!.AvatarUrl
                };

                var result = autoMapper.Map<UserProfileViewModel>(userProfile);
                await redisUserRepository.UpdateUserProfileAsync(result);

                await userServices.ApplicationDbContext.UserProfiles.AddAsync(userProfile);

                return new ApiResponseDto<UserProfileViewModel>(200, "Success", result);

            }
            else
            {

                var result = autoMapper.Map<UserProfileViewModel>(userProfile);

                await redisUserRepository.UpdateUserProfileAsync(result);
                return new ApiResponseDto<UserProfileViewModel>(200, "Success", result);
            }
        }

        return new ApiResponseDto<UserProfileViewModel>(200, "Success", dataCache);

    }

    [Authorize]
    public static async Task<ApiResponseDto<string>> UploadProfile(
           IFormFile? FormFile, [FromForm] string FirstName, [FromForm] string LastName, [FromForm] string PhoneNumber, [FromForm] string Email,
          [AsParameters] UserServices userServices, ClaimsPrincipal userAuth, [FromServices] IMinioClient minioClient, IConfiguration configuration)
    {
        ApplicationUser? user = await userServices.UserManager.FindByIdAsync(userId: userAuth.FindFirstValue("sub")!);

        if (user == null)
        {
            return new ApiResponseDto<string>(404, "User Not Found", null);
        }

        if (FormFile != null)
        {
            // check file required greater than 100kb
            if (FormFile.Length > 102400)
            {
                return new ApiResponseDto<string>(400, "File size is greater than 100kb", "");
            }

            var memoryStream = new MemoryStream();
            await FormFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            PutObjectArgs putObjectArgs = new PutObjectArgs()
                                      .WithBucket("multiappserver")
                                      .WithStreamData(memoryStream)
                                      .WithObject(FormFile.FileName)
                                      .WithObjectSize(memoryStream.Length)
                                      .WithContentType(FormFile.ContentType);


            var dataUpload = await minioClient.PutObjectAsync(putObjectArgs);

            var configSection = configuration.GetSection("MinioClient");

            var settings = new MinIoClientSettings();
            configSection.Bind(settings);
            var endpoint = settings.Endpoint;
            var fullUrl = $"https://{endpoint}/multiappserver/{FormFile.FileName}";
            user.AvatarUrl = fullUrl;
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.PhoneNumber = PhoneNumber;
            user.Email = Email;

            await userServices.UserManager.UpdateAsync(user);
            return new ApiResponseDto<string>(200, "Success", "");
        }

        return new ApiResponseDto<string>(400, "File Is Require!!!", "");
    }


    //[NonController]
    //public static async Task<bool> UpdateUserProfile()
    //{
    //    var data 
    //}
}
