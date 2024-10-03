using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BlazorApiUser.Commands.Users;

[DataContract]
public record CreateUserCommand 
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

    [Required]
    [DataMember]
    public string PasswordConfirm { get; set; }

    [Required]
    [DataMember]
    public string UserName { get; set; }

    [Required]
    [DataMember]
    public string FirstName { get; set; }

    [Required]
    [DataMember]
    public string LastName { get; set; }

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    public bool ActivateUser { get; set; } = false;

    [Required]
    public bool AutoConfirmEmail { get; set; } = false;

    //public CreateUserCommand(string email, string password, string passwordConfirm, string userName, bool rememberMe)
    //{
    //    Email = email;
    //    Password = password;
    //    PasswordConfirm = passwordConfirm;
    //    UserName = userName;
    //    RememberMe = rememberMe;
    //}
}
