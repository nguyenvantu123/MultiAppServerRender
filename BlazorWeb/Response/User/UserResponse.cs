using System.Text.Json.Serialization;

namespace BlazorWeb.Response.User
{
    public class UserResponse
    {
    }

    public class UserProfileResponse
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string PhoneNumber { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

    }

    public class UserRolesResponse
    {
        public List<UserRoleModel> UserRoles { get; set; } = new();
    }

    public class UserRoleModel
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool Selected { get; set; }
    }
}
