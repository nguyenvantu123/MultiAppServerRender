using BlazorWebApi.Users.Enums;

namespace BlazorWebApi.Request.Identity
{
    public class TokenRequest
    {

        public required string UserName { get; set; }

        public required string Password { get; set; }

        public required int ClientId { get; set; }

    }
}
