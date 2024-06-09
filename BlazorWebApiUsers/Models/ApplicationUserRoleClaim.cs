using Microsoft.AspNetCore.Identity;

namespace BlazorWebApi.Users.Models
{
    public class ApplicationUserRoleClaim : IdentityRoleClaim<Guid>
    {
        public string Description { get; set; }
        public string Group { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual ApplicationRole Role { get; set; }

        public virtual ApplicationUser User { get; set; }


        public ApplicationUserRoleClaim() : base()
        {

        }

        public ApplicationUserRoleClaim(string roleClaimDescription = null, string roleClaimGroup = null) : base()
        {
            Description = roleClaimDescription;
            Group = roleClaimGroup;
        }
    }
}