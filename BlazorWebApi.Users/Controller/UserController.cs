using AutoMapper;
using Azure;
using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.Helper;
using BlazorWebApi.Users.Queries;
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
    public class UserController : BaseApiController<UserController>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        public Guid UserId { get; set; }

        public UserController(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper, IHttpContextAccessor httpContextAccessor
)
        {
            _userManager = userManager;
            _mapper = mapper;

            UserId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToGuid() ?? Guid.Empty;
        }

        [HttpGet]
        [Route("/user/me")]
        public async Task<ResultBase<UserProfileResponse>> GetUserProfile()
        {
            var user = await _userManager.FindByIdAsync(UserId.ToString());

            var result = new ResultBase<UserProfileResponse>();

            result.Result = _mapper.Map<UserProfileResponse>(user);

            return result;
        }

        [HttpGet]
        [Route("/user")]
        public async Task<PaginatedResult<ListUserResponse>> User([FromBody] GetAllUserQuery getListUserRequest)
        {
            var user = await Mediator.Send(getListUserRequest);

            return user;
        }
    }
}
