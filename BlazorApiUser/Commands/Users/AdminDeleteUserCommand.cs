using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BlazorApiUser.Commands.Users
{
    public record AdminDeleteUserCommand : IRequest<Tuple<int, string>>
    {
        public string Id { get; set; }
    }
}
