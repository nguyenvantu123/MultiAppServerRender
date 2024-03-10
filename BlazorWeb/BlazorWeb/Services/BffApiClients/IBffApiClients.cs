using BlazorWeb.Request.Identity;
using BlazorWeb.Response.Identity;
using BlazorWeb.Response.Role;
using BlazorWeb.Response.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Polly.Retry;
using Polly;
using Refit;
using System.Net;
using Wrapper;
using Microsoft.Extensions.Configuration;
using MultiAppServer.ServiceDefaults.Wrapper;

namespace BlazorWeb.Services.BffApiClients
{
    public interface IBffApiClients
    {
        [Post("/identity/token")]
        Task<Wrapper.ResultBase<TokenResponse>> LoginAsync([Body] TokenRequest tokenRequest);

        [Post("/identity/refreshToken")]
        Task<Wrapper.ResultBase<TokenResponse>> RefreshTokenAsync([Body] RefreshTokenRequest tokenRequest);

        [Get("/user/me")]

        Task<HttpResponseMessage> UserProfile([Header("Authorization")] string authorization);

        [Post("/user/roles")]
        Task<Wrapper.ResultBase<UserRolesResponse>> GetRolesAsync();

        [Post("/roles")]
        Task<Wrapper.ResultBase<List<RoleResponse>>> GetAllRole();

    }
}
