using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class LoginWithRecoveryCodeInputModel : LoginWith2faModel
    {
        [DataType(DataType.Text)]
        public string RecoveryCode { get; set; }
    }
}
