using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BlazorWebApi.Users.Domain.Models
{
    public class UserRole : IdentityUserRole<Guid>
    {

        public override Guid UserId { get; set; }
        
        public override Guid RoleId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}
