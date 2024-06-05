using BlazorWebApi.Bff.Wrapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace BlazorWebApi.Bff.ApiClients
{
    public interface IUserApiClient
    {

        [Get("/user/me")]
        public Task<object> UserProfile();

        [Get("/user")]
        public Task<object> GetAllUser([FromBody] GetAllUserQuery getAllUserQuery);

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

    public class GetAllUserQuery
    {
        public string SearchText { get; set; }

        public List<Guid> RoleIds { get; set; }

        public bool? IsActive { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public string[] OrderBy { get; set; }
    }

}
