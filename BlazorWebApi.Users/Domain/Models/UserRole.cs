using Microsoft.AspNetCore.Identity;


namespace BlazorWebApi.Users.Domain.Models
{
    public class UserRole : IdentityRole
    {
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual ICollection<UserRoleClaim> RoleClaims { get; set; }

        public UserRole() : base()
        {
            RoleClaims = new HashSet<UserRoleClaim>();
        }

        public UserRole(string roleName, string roleDescription = null) : base(roleName)
        {
            RoleClaims = new HashSet<UserRoleClaim>();
            Description = roleDescription;
        }
    }
}