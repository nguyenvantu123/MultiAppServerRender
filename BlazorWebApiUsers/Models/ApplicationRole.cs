namespace BlazorWebApi.Users.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {

        //public new Guid Id { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual ICollection<ApplicationUserRoleClaim> RoleClaims { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

        public ApplicationRole() : base()
        {
            RoleClaims = new HashSet<ApplicationUserRoleClaim>();
        }

        public ApplicationRole(string roleName, string roleDescription = null) : base(roleName)
        {
            RoleClaims = new HashSet<ApplicationUserRoleClaim>();
            Description = roleDescription;
        }
    }
}