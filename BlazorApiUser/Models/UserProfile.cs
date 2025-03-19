using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Finbuckle.MultiTenant;

namespace BlazorApiUser.Models
{
    [MultiTenant]
    public partial class UserProfile
    {
        [Key]
        public Guid Id { get; set; } = new Guid();

        public Guid UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string LastPageVisited { get; set; } = "/";
        public bool IsNavOpen { get; set; } = true;
        public bool IsNavMinified { get; set; } = false;
        public int Count { get; set; } = 0;
        public DateTime LastUpdatedDate { get; set; } = DateTime.MinValue;
        public string Culture { get; set; }

        public bool IsDarkMode { get; set; } = false;

        [Column(TypeName = "nvarchar(64)")]
        public string TenantId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
