namespace BlazorWebApi.Users.Constants
{
    public static class Constant
    {
        public const string SuperAdministratorRole = "SuperAdministratorRole";
        public const string BasicUserRole = "BasicUserRole";
        public const string DefaultPassword = "123Pa$$word!";
    }

    public static class UserConstants
    {
        public const string DefaultPassword = "Abc@12345";
    }

    public static class DefaultTenant
    {
        public const string DefaultTenantId = "Master";
    }

    public static class DefaultUserNames
    {
        public const string Administrator = "admin";
        public const string User = "user";
    }

    public static class DefaultRoleNames
    {
        public const string Administrator = "Administrator";
    }

    public static class AuthenPath
    {
        public const string LoginPath = "/account/login";
        public const string LoginWith2faPath = "/acount/loginwith2fa";
        public const string LoginWithRecoveryCodePath = "/account/loginwithrecoverycode";
        public const string ProfilePath = "/account/profile";
        public const string DefaultTenantId = "Master";
    }
}