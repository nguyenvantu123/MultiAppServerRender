using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BlazorApiUser.Commands.Users;

public record CreateUserCommand : IRequest<Tuple<int, string>>
{

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


    public bool RememberMe { get; set; }
    public CreateUserCommand(string email, string password, string passwordConfirm, string userName, bool rememberMe)
    {
        Email = email;
        Password = password;
        PasswordConfirm = passwordConfirm;
        UserName = userName;
        RememberMe = rememberMe;
    }
}
