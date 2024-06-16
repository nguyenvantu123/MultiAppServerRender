using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class LoginWith2faModel : AccountFormModel
    {
        [Display(Name = "RememberBrowser")]
        public bool RememberMachine { get; set; }
    }
}
