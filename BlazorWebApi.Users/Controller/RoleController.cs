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
    public class RoleController : ControllerBase
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly IMapper _mapper;

        public Guid UserId { get; set; }

        public RoleController(UserManager<User> userManager, RoleManager<UserRole> roleManager, IMapper mapper, IHttpContextAccessor httpContextAccessor
)
        {
            _userManager = userManager;
            _mapper = mapper;

            UserId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToGuid() ?? Guid.Empty;
        }

    }
}
