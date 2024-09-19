using BlazorIdentity.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace BlazorApiUser.Queries.Roles
{
    public record GetListRoleQuery(int pageSize = 10, int pageNumber = 0) : IRequest<Tuple<int, List<RoleDto>>>;

}
