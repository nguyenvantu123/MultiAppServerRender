using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorIdentity.Users.Models
{
    [MultiTenant]
    //[Permissions(Actions.CRUD)]
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
        public override Guid UserId { get => base.UserId; set => base.UserId = value; }
        public override Guid RoleId { get => base.RoleId; set => base.RoleId = value; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
