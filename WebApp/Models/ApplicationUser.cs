using BlazorIdentity.Users.Models;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Components.Pages.Admin;
using WebApp.Permissions;

namespace WebApp.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    //[Table("ApplicationUser")]
    [MultiTenant]
    [Permissions(Actions.CRUD)]
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid Id = new Guid();
        public override string UserName { get => base.UserName; set => base.UserName = value; }
        public override string NormalizedUserName { get => base.NormalizedUserName; set => base.NormalizedUserName = value; }
        public override string Email { get => base.Email; set => base.Email = value; }
        public override string NormalizedEmail { get => base.NormalizedEmail; set => base.NormalizedEmail = value; }
        public override bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }
        public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }
        public override bool PhoneNumberConfirmed { get => base.PhoneNumberConfirmed; set => base.PhoneNumberConfirmed = value; }
        public override bool TwoFactorEnabled { get => base.TwoFactorEnabled; set => base.TwoFactorEnabled = value; }
        public override DateTimeOffset? LockoutEnd { get => base.LockoutEnd; set => base.LockoutEnd = value; }
        public override bool LockoutEnabled { get => base.LockoutEnabled; set => base.LockoutEnabled = value; }
        public override int AccessFailedCount { get => base.AccessFailedCount; set => base.AccessFailedCount = value; }
        public override string ConcurrencyStamp { get => base.ConcurrencyStamp; set => base.ConcurrencyStamp = value; }

        public string LastName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedOn { get; set; }

        public string DeletedBy { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public string FirstName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public ICollection<ApiLogItem> ApiLogItems { get; set; }

        public UserProfile Profile { get; set; }

        //public virtual ICollection<Message> Messages { get; set; }

        public string? AvatarUrl { get; set; }

        public string UserType { get; set; }
    }
}
