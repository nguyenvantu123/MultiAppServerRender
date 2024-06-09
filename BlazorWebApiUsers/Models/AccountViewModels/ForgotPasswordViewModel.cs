namespace BlazorWebApi.Users.Models.AccountViewModels
{
    public record ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; }
    }
}
