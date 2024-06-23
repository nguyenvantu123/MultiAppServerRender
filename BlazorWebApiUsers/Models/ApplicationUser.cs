using BlazorBoilerplate.Infrastructure.Storage.DataModels;
using BlazorBoilerplate.Infrastructure.Storage.Permissions;
using Finbuckle.MultiTenant;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorWebApi.Users.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    [Table("ApplicationUser")]
    //[MultiTenant]
    //[Permissions(Actions.CRUD)]
    public class ApplicationUser : IdentityUser<Guid>
    {
        public new Guid Id { get; set; }
        public string LastName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public string FirstName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public ICollection<ApiLogItem> ApiLogItems { get; set; }

        public UserProfile Profile { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public string TenantId { get; set; }

    }
}
