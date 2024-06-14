using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class LoginInputModel : AccountFormModel
    {
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
