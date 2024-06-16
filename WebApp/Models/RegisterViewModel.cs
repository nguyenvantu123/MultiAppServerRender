using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class RegisterViewModel : LoginInputModel
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string PasswordConfirm { get; set; }
    }
}
