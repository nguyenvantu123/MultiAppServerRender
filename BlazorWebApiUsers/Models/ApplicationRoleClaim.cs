using System;
using Microsoft.AspNetCore.Identity;

namespace BlazorWebApi.Users.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}
