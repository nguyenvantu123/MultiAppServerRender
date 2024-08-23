using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BlazorApiUser.Commands.Users
{
    public record ChangePasswordCommand : IRequest<Tuple<int, string>>
    {

        public string UserId { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }
}