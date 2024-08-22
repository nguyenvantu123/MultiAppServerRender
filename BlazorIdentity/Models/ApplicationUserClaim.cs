using Finbuckle.MultiTenant;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorIdentity.Users.Models
{
    [MultiTenant]
    [Permissions(Actions.CRUD)]
    public class ApplicationUserClaim : IdentityUserClaim<Guid>
    {

        public override Guid UserId { get; set; }
    }
}
