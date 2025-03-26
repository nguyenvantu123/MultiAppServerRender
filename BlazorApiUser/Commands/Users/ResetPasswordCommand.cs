using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BlazorApiUser.Commands.Users
{
    public class ResetPasswordCommand
    {

        [Required]
        public string PassToken { get; set; }

        [DataMember]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; init; }

        [Required]
        [DataMember]
        public string PasswordConfirm { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string UserId { get; set; }
    }
}
