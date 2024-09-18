using BlazorIdentity.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace BlazorApiUser.Queries.Roles
{
    public record GetListRoleQuery : IRequest<Tuple<int, List<RoleDto>>>
    {
        public int PageSize = 10;

        public int PageNumber = 0;

        public GetListRoleQuery(int pageSize, int pageNumber)
        {
            pageSize = PageSize;
            pageNumber = PageNumber;
        }
    }


}
