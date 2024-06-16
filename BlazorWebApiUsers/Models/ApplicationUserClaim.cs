using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorWebApi.Users.Models
{
    [Table("ApplicationUserClaim")]
    public class ApplicationUserClaim : IdentityUserClaim<Guid>
    {

        public override Guid UserId { get; set; }
    }
}
