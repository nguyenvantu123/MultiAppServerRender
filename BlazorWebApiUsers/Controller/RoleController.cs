using AutoMapper;
using Azure;
using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.Helper;
using BlazorWebApi.Users.Request.User;
using BlazorWebApi.Users.Response.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization;
using System.Security.Claims;

namespace BlazorWebApi.Users.Controller
{

    [Authorize]
    public class RoleController : BaseApiController<RoleController>
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        public Guid UserId { get; set; }

        public RoleController(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper, IHttpContextAccessor httpContextAccessor
)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;

            UserId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToGuid() ?? Guid.Empty;
        }

        [HttpGet]
        [Route("/role")]
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
