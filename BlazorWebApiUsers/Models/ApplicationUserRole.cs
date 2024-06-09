using System.ComponentModel.DataAnnotations.Schema;
using eShop.Identity.API.Models;
using Microsoft.AspNetCore.Identity;

namespace BlazorWebApi.Users.Domain.Models
{
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {

        public override Guid UserId { get; set; }
        
        public override Guid RoleId { get; set; }
    }
}
