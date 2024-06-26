﻿using Microsoft.AspNetCore.Identity;
using ServiceDefaults;
using System.ComponentModel.DataAnnotations.Schema;
//using BlazorWeb.Contracts;

namespace BlazorWebApi.Users.Domain.Models
{

    public class User : IdentityUser<Guid>, IAuditableEntity
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string CreatedBy { get; set; }

        [Column(TypeName = "text")]
        public string ProfilePictureDataUrl { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        //public virtual ICollection<ChatHistory<BlazorHeroUser>> ChatHistoryToUsers { get; set; }

        //public User()
        //{
        //    //ChatHistoryFromUsers = new HashSet<ChatHistory<BlazorHeroUser>>();
        //    //ChatHistoryToUsers = new HashSet<ChatHistory<BlazorHeroUser>>();
        //}
    }
}
