namespace BlazorIdentity.Users.Models
{
    public class ConfirmEmailViewModel
    {
        [Required(ErrorMessage = "FieldRequired")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        public string Token { get; set; }
    }
}
