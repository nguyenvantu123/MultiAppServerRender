using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class AuthenticatorVerificationCodeViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "VerificationCode")]
        public string Code { get; set; }
    }
}
