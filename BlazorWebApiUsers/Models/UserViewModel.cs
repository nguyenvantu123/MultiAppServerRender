namespace BlazorWebApi.Users.Models
{
    public class UserViewModel : BaseDto
    {
        public bool IsAuthenticated { get; set; }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string TenantId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool HasPassword { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool HasAuthenticator { get; set; }
        public List<KeyValuePair<string, string>> Logins { get; set; }
        public bool BrowserRemembered { get; set; }
        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }
        public string[] RecoveryCodes { get; set; }
        public int CountRecoveryCodes { get; set; }
        public List<string> Roles { get; set; }
        public List<KeyValuePair<string, string>> ExposedClaims { get; set; }

        public List<UserRoleViewModel> UserRoles { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool IsActive { get; set; }
    }

    public class UserRoleViewModel
    {

        public Guid RoleId { get; set; }
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

    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; }
        public IList<UserRoleModel> UserRoles { get; set; }
    }
}

