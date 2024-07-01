namespace WebApp.Models
{
    public class LoginResponseModel
    {
        public bool RequiresTwoFactor { get; set; }

        public string LastPageVisited { get; set; }
    }
}
