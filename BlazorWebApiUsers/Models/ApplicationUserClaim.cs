namespace BlazorWebApi.Users.Models
{
    public class ApplicationUserClaim : IdentityUserClaim<Guid>
    {

        public override Guid UserId { get; set; }
    }
}
