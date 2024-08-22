namespace BlazorIdentity.Users.Models.AccountViewModels
{
    public record ResetPasswordViewModel
    {
        public string UserId { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public string Token { get; set; }
    }
}
