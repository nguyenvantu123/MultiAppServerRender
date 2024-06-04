namespace BlazorWeb.Request.Identity
{
    public class RefreshTokenRequest
    {
        public required string accessToken { get; set; }

        public required string refreshToken { get; set; }
    }
}
