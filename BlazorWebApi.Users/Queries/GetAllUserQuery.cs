using Amazon.Runtime.Documents;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Specifications.Base;
using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.Extensions;
using BlazorWebApi.Users.Repository;
using BlazorWebApi.Users.Request.User;
using BlazorWebApi.Users.Response.User;
using BlazorWebApi.Users.Specifications;
using LazyCache;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using MultiAppServer.ServiceDefaults.Wrapper;
using System.Linq;
using System.Linq.Expressions;

namespace BlazorWebApi.Users.Queries
{
    public class GetAllUserQuery : IRequest<PaginatedResult<ListUserResponse>>
    {
        public string SearchText { get; set; }

        public List<Guid> RoleIds { get; set; }

        public bool? IsActive { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public string[] OrderBy { get; set; }

        public GetAllUserQuery(string searchText, List<Guid> roleIds, bool? isActive, int pageSize, int pageNumber, string orderBy)
        {
            SearchText = searchText;
            RoleIds = roleIds;
            IsActive = isActive;
            PageNumber = pageNumber;
            PageSize = pageSize;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            };
        }
    }

    internal class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, PaginatedResult<ListUserResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllUserQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PaginatedResult<ListUserResponse>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
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
                RoleName = string.Join(";", e.UserRoles.Select(x => x.Role.Name).ToList()),
            };

            var userFilterSpec = new UserSpecification(request);

            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<User>().Entities
                   .Specify(userFilterSpec)
                   .Select(expression)
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
                var data = await _unitOfWork.Repository<User>().Entities
                   .Specify(userFilterSpec)
                   .OrderBy(ordering) // require system.linq.dynamic.core
                   .Select(expression)
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;

            }
        }
    }
}
