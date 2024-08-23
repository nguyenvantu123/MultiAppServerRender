using BlazorIdentity.Users.Models;
using MediatR;

namespace BlazorApiUser.Commands.Users
{
    public record UpdateUserRolesCommand : IRequest<Tuple<int, string>>
    {
        public string UserId { get; set; }
        public IList<UserRoleModel> UserRoles { get; set; }
    }
}
