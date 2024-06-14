using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class LoginWith2faInputModel : LoginWith2faModel
    {
        [DataType(DataType.Text)]
        public string TwoFactorCode { get; set; }
    }
}
