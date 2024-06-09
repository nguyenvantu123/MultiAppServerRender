namespace BlazorWebApi.Users.Models
{
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {

        public override Guid UserId { get; set; }
        
        public override Guid RoleId { get; set; }
    }
}
