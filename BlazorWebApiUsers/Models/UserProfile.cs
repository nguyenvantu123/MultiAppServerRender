using System.ComponentModel.DataAnnotations.Schema;
using Finbuckle.MultiTenant;

namespace BlazorWebApi.Users.Models
{
    [MultiTenant]
    public partial class UserProfile
    {
        [Key]
        public long Id { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string LastPageVisited { get; set; } = "/";
        public bool IsNavOpen { get; set; } = true;
        public bool IsNavMinified { get; set; } = false;
        public int Count { get; set; } = 0;
        public DateTime LastUpdatedDate { get; set; } = DateTime.MinValue;
        public string Culture { get; set; }

        [Column(TypeName = "nvarchar(64)")]
        public string TenantId { get; set; }
    }
}
