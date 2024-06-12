namespace BlazorWebApi.Users.Data
{
    public class CustomAuthService
    {
        public CustomAuthService() 
        {
            Users = new Dictionary<string, ClaimsPrincipal>();


        }  

        public Dictionary<string, ClaimsPrincipal> Users { get; set; }
    }
}
