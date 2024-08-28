using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BlazorApiUser.Commands.Users;

[DataContract]
public record CreateUserCommand : IRequest<Tuple<int, string>>
{

    [DataMember]
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; init; }

    [DataMember]
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; init; }

    [DataMember]
    public string PasswordConfirm { get; set; }

    [DataMember]
    public string UserName { get; set; }

    [DataMember]
    public bool RememberMe { get; set; }
    //public CreateUserCommand(string email, string password, string passwordConfirm, string userName, bool rememberMe)
    //{
    //    Email = email;
    //    Password = password;
    //    PasswordConfirm = passwordConfirm;
    //    UserName = userName;
    //    RememberMe = rememberMe;
    //}
}
