using BlazorIdentity.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;

namespace BlazorApiUser.Queries.Users
{
    [DataContract]
    public record GetListUserQuery : IRequest<Tuple<int, List<UserDataViewModel>>>
    {
        [DataMember]
        public int PageSize = 10;

        [DataMember]
        public int PageNumber = 0;


    }
}
