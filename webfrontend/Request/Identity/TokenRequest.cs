using Shared.Enums;

namespace BlazorWeb.Request.Identity
{
    public class TokenRequest
    {

        public string UserName { get; set; }

        public string Password { get; set; }

        public int ClientId { get; set; }

        public bool IsRemember { get; set; }

    }
}
