namespace WebApp
{
    public static class ApplicationClaimTypes
    {
        ///<summary>A claim that specifies the permission of an entity</summary>
        public const string Permission = "permission";
        public static string EmailVerified = "email_verified";
    }

    public static class ClaimValues
    {
        public static string TrueString = "true";
        public static string FalseString = "false";

        public static string AuthenticationMethodMfa = "mfa";
        public static string AuthenticationMethodPwd = "pwd";
    }
}