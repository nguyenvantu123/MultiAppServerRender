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
using Microsoft.Extensions.Configuration;
using BlazorWeb.Wrapper;

namespace BlazorWeb.Services.BffApiClients
{
    public interface IBffApiClients
    {
        [Post("/identity/token")]
        Task<ResultBase<TokenResponse>> LoginAsync([Body] TokenRequest tokenRequest);

        [Post("/identity/refreshToken")]
        Task<ResultBase<TokenResponse>> RefreshTokenAsync([Body] RefreshTokenRequest tokenRequest);

        [Get("/user/me")]
        Task<ResultBase<UserProfileResponse>> UserProfile([Header("Authorization")] string authorization);

        [Post("/user/roles")]
        Task<ResultBase<UserRolesResponse>> GetRolesAsync();

        [Post("/roles")]
        Task<ResultBase<List<RoleResponse>>> GetAllRole();

    }
}
