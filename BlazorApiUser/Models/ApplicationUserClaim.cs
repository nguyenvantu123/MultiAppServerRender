using Finbuckle.MultiTenant;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApiUser.Models
{
    [MultiTenant]
    [Permissions(Actions.CRUD)]
    public class ApplicationUserClaim : IdentityUserClaim<Guid>, ISoftDelete
    {

        public override Guid UserId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
