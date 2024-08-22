namespace BlazorIdentity.Users.Models.AccountViewModels
{
    public record RegisterViewModel
    {
        //[Required]
        //[EmailAddress]
        //[Display(Name = "Email")]
        //public string Email { get; init; }

        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "Password")]
        //public string Password { get; init; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; init; }

        //public ApplicationUser User { get; init; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; init; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; init; }

        public string PasswordConfirm { get; set; }

        public string UserName { get; set; }

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
