using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;

namespace BlazorApiUser.Commands.Users
{
    public class UpdateProfileCommand
    {
        public IFormFile Avatar { get; set; }

        public string FirstName { get; set; }


        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
