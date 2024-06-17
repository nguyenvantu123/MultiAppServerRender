namespace BlazorWebApi.Users.Models.ManageViewModels
{
    public record ChangePasswordViewModel
    {
        public string UserId { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }
}
