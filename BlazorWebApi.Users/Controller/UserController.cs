using AutoMapper;
using Azure;
using BlazorWebApi.Identity.RoleConst;
using BlazorWebApi.Identity.UserConst;
using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.Helper;
using BlazorWebApi.Users.Request.User;
using BlazorWebApi.Users.Response.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiAppServer.ServiceDefaults.Wrapper;
using System.Runtime.Serialization;
using System.Security.Claims;

namespace BlazorWebApi.Users.Controller
{
    [Authorize]
    public class UserController : ControllerBase
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly IMapper _mapper;

        public Guid UserId { get; set; }

        public UserController(UserManager<User> userManager, RoleManager<UserRole> roleManager, IMapper mapper, IHttpContextAccessor httpContextAccessor
)
        {
            _userManager = userManager;
            _mapper = mapper;

            UserId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToGuid() ?? Guid.Empty;
        }

        //[HttpPost]
        //[Route("/user/register")]
        //public async Task<IResult> Register(RegisterUserRequest model)
        //{
        //    var user = await _userManager.FindByNameAsync(model.UserName);
        //    if (user != null)
        //    {
        //        return await Result.FailAsync("User Name Is Existed.");
        //    }
        //    user = await _userManager.FindByEmailAsync(model.Email);

        //    if (user != null)
        //    {
        //        return await Result.FailAsync(message: "Email Is Existed.");
        //    }

        //    user = await _userManager.Users.Where(x => x.PhoneNumber == model.PhoneNumber).FirstOrDefaultAsync();

        //    if (user != null)
        //    {
        //        return await Result.FailAsync("Phone Number Is Existed.");
        //    }

        //    if (model.Password != model.VerifyPassword)
        //    {
        //        return await Result.FailAsync("Password and verify password are not match.");
        //    }

        //    var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        //    if (!passwordValid)
        //    {
        //        return await Result.FailAsync("Password required length 8 digit, at least 1 uppercase and 1 special character.");
        //    }

        //    //await _userManager.CreateAsync(model);

        //    var userCreate = _mapper.Map<User>(model);

        //    await _userManager.CreateAsync(userCreate, model.Password);

        //    var role = await _userManager.AddToRoleAsync(userCreate, RoleConstants.BasicUserRole);


        //    return await Result.SuccessAsync(message: "Success");
        //}

        [HttpGet]
        [Route("/user/me")]
        public async Task<ResultBase<UserProfileResponse>> GetUserProfile()
        {
            var user = await _userManager.FindByIdAsync(UserId.ToString());

            var result = new ResultBase<UserProfileResponse>();

            result.Result = _mapper.Map<UserProfileResponse>(user);

            return result;
        }

        [HttpGet("roles")]
        public async Task<UserRolesResponse> GetRolesAsync()
        {
            var viewModel = new List<UserRoleModel>();

            var user = await _userManager.FindByIdAsync(UserId.ToString());
            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRoleModel
                {
                    RoleName = role.Name,
                    RoleDescription = role.Description
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
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

            return result;
        }

    }
}
