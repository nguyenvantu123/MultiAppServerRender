using BlazorWebApi.Bff.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace BlazorWebApi.Bff.ApiClients
{
    public interface IUserApiClient
    {

        [Get("/user/me")]
        public Task<dynamic> UserProfile();

    }

    public record class UserRolesResponse
    {
        public List<UserRoleModel> UserRoles { get; set; } = new();
    }

    public record UserProfile
    {
        public string Email { get; set; }
        public string FirstName { get; set; }

        public string PhoneNumber { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

    };

    public record class UserRoleModel
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool Selected { get; set; }
    }

}
