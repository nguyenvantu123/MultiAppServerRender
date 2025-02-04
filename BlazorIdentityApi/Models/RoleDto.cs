using System.ComponentModel.DataAnnotations;

namespace BlazorIdentityApi.Models
{
    public class RoleDto
    {
        [StringLength(64, ErrorMessage = "ErrorInvalidLength", MinimumLength = 2)]
        [RegularExpression(@"[^\s]+", ErrorMessage = "SpacesNotPermitted")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        //public List<string> Permissions { get; set; }

        //public string FormattedPermissions
        //{
        //    get
        //    {
        //        return String.Join(", ", Permissions.ToArray());
        //    }
        //}

        public Guid Id { get; set; }
    }
}
