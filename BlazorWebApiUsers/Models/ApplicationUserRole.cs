using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorWebApi.Users.Models
{
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public new Guid Id { get; set; }
        public override Guid UserId { get; set; }
        
        public override Guid RoleId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("RoleId")]
        public virtual ApplicationRole Role { get; set; }

    }
}
