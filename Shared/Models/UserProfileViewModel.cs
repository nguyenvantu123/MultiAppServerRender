using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class UserProfileViewModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string LastPageVisited { get; set; } = "/";
        public bool IsNavOpen { get; set; } = true;
        public bool IsNavMinified { get; set; } = false;
        public int Count { get; set; } = 0;
        public DateTime LastUpdatedDate { get; set; }
        public string Culture { get; set; }

        public bool IsDarkMode { get; set; } = false;

        [Column(TypeName = "nvarchar(64)")]
        public string TenantId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        public string? AvatarUrl { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string UserName { get; set; }
    }
}
