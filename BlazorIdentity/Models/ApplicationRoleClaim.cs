﻿using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorIdentity.Users.Models
{
    [MultiTenant]
    [Permissions(Actions.CRUD)]
    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        public string Description { get; set; }
        public string Group { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        [ForeignKey("RoleId")]
        public virtual ApplicationRole Role { get; set; }

        public virtual ApplicationUser User { get; set; }


        public ApplicationRoleClaim() : base()
        {

        }

        public ApplicationRoleClaim(string roleClaimDescription = null, string roleClaimGroup = null) : base()
        {
            Description = roleClaimDescription;
            Group = roleClaimGroup;
        }
    }
}