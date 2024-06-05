using System.Linq.Expressions;
using System.Runtime.Serialization;
using AutoMapper;
using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.Extensions;
using BlazorWebApi.Users.Repository;
using BlazorWebApi.Users.Response.User;
using BlazorWebApi.Users.Specifications;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoleConstants = BlazorWebApi.Users.RoleConst.RoleConstants;

namespace BlazorWebApi.Users.Queries;

public class GetUserByIdQuery : IRequest<ListUserResponse>
{
    public Guid Id { get; init; }
}

public class GetUserByIdQueryHandler(IUnitOfWork<Guid> unitOfWork, IMapper mapper, IAppCache cache)
    : IRequestHandler<GetUserByIdQuery, ListUserResponse>
{
    private readonly IAppCache _cache = cache;

    public async Task<ListUserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<User, ListUserResponse>> expression = e => new ListUserResponse
        {
            Id = e.Id,
            FirstName = e.FirstName,
            UserName = e.UserName,
            Email = e.Email,
            LastName = e.LastName,
            PhoneNumber = e.PhoneNumber,
            CreationTime = e.CreatedOn,
            IsActive = e.IsActive,
            RoleIds = e.UserRoles.Select(x => x.Role.Id).ToList(),
            IsAdmin = e.UserRoles.FirstOrDefault(x => x.Role.Name == RoleConstants.SuperAdministratorRole) != null
                ? true
                : false
        };

        var userFilterSpec = new UserIdSpecification(request);

        var data = await unitOfWork.Repository<User>().Entities
            .Specify(userFilterSpec)
            .Select(expression).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return data;
    }
}