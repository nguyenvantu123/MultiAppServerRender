using BlazorIdentity.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;

namespace BlazorApiUser.Queries.Users
{
    public record GetListUserQuery(int pageNumber = 0, int pageSize = 10) : IRequest<Tuple<int, List<UserDataViewModel>>>;
}
