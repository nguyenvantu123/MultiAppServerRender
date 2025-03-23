using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BlazorApiUser.Commands.Users
{
    public record ChangePasswordCommand 
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password Confirm is required")]
        public string NewPasswordConfirm { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "old Password is required")]
        public string CurrentPassword { get; set; }
    }
}