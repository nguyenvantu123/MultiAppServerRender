using BlazorApiUser.Models;
using MediatR;

namespace BlazorApiUser.Commands.Users
{
    public record UpdateUserRolesCommand
    {
        public List<UserRoleModel> UserRoles { get; set; }
    }
}
