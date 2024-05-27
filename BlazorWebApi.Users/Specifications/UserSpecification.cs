using BlazorHero.CleanArchitecture.Application.Specifications.Base;
using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.Queries;

namespace BlazorWebApi.Users.Specifications
{
    public class UserSpecification : BaseSpecification<User>
    {
        public UserSpecification(GetAllUserQuery searchQuery)
        {

            Includes.Add(x => x.UserRoles);

            if (!string.IsNullOrEmpty(searchQuery.SearchText))
            {
                Criteria = p => p.UserName.Contains(searchQuery.SearchText) || p.PhoneNumber.Contains(searchQuery.SearchText) || p.Email.Contains(searchQuery.SearchText);
            }
            if (searchQuery.IsActive.HasValue)
            {
                Criteria = p => p.IsActive == searchQuery.IsActive;
            }

            if (searchQuery.RoleIds != null && searchQuery.RoleIds.Any())
            {
                Criteria = p => p.UserRoles.Any(x=> searchQuery.RoleIds.Contains(x.Role.Id));
            }
        }
    }
}
