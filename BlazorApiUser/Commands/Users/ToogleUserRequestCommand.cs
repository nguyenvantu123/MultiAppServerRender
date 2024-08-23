using MediatR;

namespace BlazorApiUser.Commands.Users
{
    public record ToogleUserRequestCommand : IRequest<Tuple<int, string>>
    {
        public bool ActivateUser { get; set; }
        public string UserId { get; set; }
    }
}
