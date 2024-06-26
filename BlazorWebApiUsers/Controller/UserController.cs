﻿using AutoMapper;
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
using System.Runtime.Serialization;
using System.Security.Claims;
using MediatR;
using ServiceDefaults;

namespace BlazorWebApi.Users.Controller
{
    [Authorize]
    public class UserController : BaseApiController<UserController>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private Guid _userId;

        private Guid GetUserId() => _userId;

        private void SetUserId(Guid value) => _userId = value;

        public UserController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IMapper mapper, IHttpContextAccessor httpContextAccessor
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;

            if (httpContextAccessor.HttpContext != null)
                SetUserId(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToGuid() ??
                          Guid.Empty);
        }

        [HttpGet]
        [Route("/user/me")]
        public async Task<ResultBase<UserProfileResponse>> GetUserProfile()
        {
            var user = await _userManager.FindByIdAsync(GetUserId().ToString());

            var result = new ResultBase<UserProfileResponse>();

            result.Result = _mapper.Map<UserProfileResponse>(user);

            return result;
        }

        [HttpGet]
        [Route("/user")]
        public async Task<PaginatedResult<ListUserResponse>> GetUser([FromQuery] GetAllUserQuery getListUserRequest)
        {
            // getListUserRequest ??= new GetAllUserQuery();
            var user = await Mediator.Send(getListUserRequest);

            return user;
        }

        [HttpGet]
        [Route("/user/{id}")]
        public async Task<ListUserResponse> GetUserById([FromRoute] Guid userId)
        {
            var getUserById = new GetUserByIdQuery
            {
                Id = userId
            };
            var user = await Mediator.Send(getUserById);

            return user;
        }
    }
}