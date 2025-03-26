using BlazorApiUser.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApiUser.Queries.Roles
{
    public record GetListRoleQuery(int pageSize = 10, int pageNumber = 0, string name = "") : IRequest<Tuple<int, List<RoleDto>>>;

}
