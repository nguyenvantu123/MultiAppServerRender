namespace BlazorWebApi.Users.Models
{
    public class LoginWithRecoveryCodeInputModel
    {
        [DataType(DataType.Text)]
        public string RecoveryCode { get; set; }

        private string returnUrl;

        [Display(Name = "RememberBrowser")]
        public bool RememberMachine { get; set; }

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
