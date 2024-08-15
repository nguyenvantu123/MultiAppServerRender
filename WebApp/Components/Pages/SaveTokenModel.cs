using System.ComponentModel.DataAnnotations;
using WebApp.Models;

namespace WebApp.Pages
{
    public class SaveTokenModel : AccountFormModel
    {
        public string Access_Token { get; set; }

        public string Refresh_Token { get; set; }

        public DateTime Expire_In { get; set; }

    }
}
