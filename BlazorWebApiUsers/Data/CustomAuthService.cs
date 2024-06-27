namespace BlazorWebApi.Users.Data
{
    public class CustomAuthService
    {
        public Dictionary<string, ClaimsPrincipal> Users { get; set; } = new();
    }
}
