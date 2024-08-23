using BlazorIdentity.Users.Models;
using MediatR;
using WebApp.Models;

namespace BlazorApiUser.Queries.Roles
{
    public record GetListRoleQuery : IRequest<Tuple<int, List<RoleDto>>>
    {

        public int pageSize = 10;

        public int pageNumber = 0;

        public string search = "";
    }
}
