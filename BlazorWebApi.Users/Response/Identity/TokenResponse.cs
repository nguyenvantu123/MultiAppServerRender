namespace BlazorWebApi.Users.Response.Identity
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime ExpireIn { get; set; }
    }
}
