using Microsoft.AspNetCore.Identity;


namespace BlazorWebApi.Users.Domain.Models
{
    public class Role : IdentityRole<Guid>
    {
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual ICollection<UserRoleClaim> RoleClaims { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public Role() : base()
        {
            RoleClaims = new HashSet<UserRoleClaim>();
        }

        public Role(string roleName, string roleDescription = null) : base(roleName)
        {
            RoleClaims = new HashSet<UserRoleClaim>();
            Description = roleDescription;
        }
    }
}