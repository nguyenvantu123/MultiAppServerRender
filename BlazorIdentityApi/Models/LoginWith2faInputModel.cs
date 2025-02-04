using System.ComponentModel.DataAnnotations;

namespace BlazorIdentity.Users.Models
{
    public class LoginWith2faInputModel 
    {
        [DataType(DataType.Text)]
        public string TwoFactorCode { get; set; }

        [Display(Name = "RememberBrowser")]
        public bool RememberMachine { get; set; }

        private string returnUrl;

        public bool RememberMe { get; set; }

        public string ReturnUrl
        {
            get => returnUrl ?? "/";
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.StartsWith("http"))
                        value = new Uri(value).LocalPath;

                    if (!value.StartsWith("/"))
                        value = $"/{value}";
                }

                returnUrl = value;
            }
        }

        public string __RequestVerificationToken { get; set; }
    }
}
