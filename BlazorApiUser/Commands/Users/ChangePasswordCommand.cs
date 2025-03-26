using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BlazorApiUser.Commands.Users
{
    public record ChangePasswordCommand 
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "New Password is required")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password Confirm is required")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string NewPasswordConfirm { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Current Password is required")]
        public string CurrentPassword { get; set; }


        public bool? LogoutAllDevice { get; set; }
    }
}