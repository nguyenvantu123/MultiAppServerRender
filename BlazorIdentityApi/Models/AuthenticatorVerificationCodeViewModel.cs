using System.ComponentModel.DataAnnotations;

namespace BlazorIdentity.Users.Models
{
    public class AuthenticatorVerificationCodeViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "VerificationCode")]
        public string Code { get; set; }
    }
}
