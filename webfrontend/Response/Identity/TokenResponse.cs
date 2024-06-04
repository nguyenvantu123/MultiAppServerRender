namespace BlazorWeb.Response.Identity
{
    public class TokenResponse
    {

        public required string AccessToken { get; set; }

        public required string RefreshToken { get; set; }

        public DateTime ExpireIn { get; set; }

        public string? ReturnUrl { get; set; }

    }
}
