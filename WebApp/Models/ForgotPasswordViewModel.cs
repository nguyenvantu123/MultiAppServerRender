using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class ForgotPasswordViewModel
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
