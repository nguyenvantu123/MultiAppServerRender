using BlazorIdentity.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApiUser.Queries.Users
{
    public record GetListUserQuery : IRequest<Tuple<int, List<UserDataViewModel>>>
    {

        public int pageSize = 10;

        public int pageNumber = 0;
    }
}
