using Microsoft.AspNetCore.Identity;

namespace BlazorWebApi.Users.Domain.Models
{
    public class UserRoleClaim : IdentityRoleClaim<Guid>
    {
        public string Description { get; set; }
        public string Group { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual Role Role { get; set; }

        public UserRoleClaim() : base()
        {

        }

        public UserRoleClaim(string roleClaimDescription = null, string roleClaimGroup = null) : base()
        {
            Description = roleClaimDescription;
            Group = roleClaimGroup;
        }
    }
}