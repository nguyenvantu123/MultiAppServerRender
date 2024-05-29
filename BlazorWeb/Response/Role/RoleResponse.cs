using System.ComponentModel.DataAnnotations;

namespace BlazorWeb.Response.Role
{
    public class RoleResponse
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
