using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.Queries;
using BlazorWebApi.Users.Specifications.Base;

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
    
    public class UserIdSpecification : BaseSpecification<User>
    {
        public UserIdSpecification(GetUserByIdQuery getUserByIdQuery)
        {

            Includes.Add(x => x.UserRoles);
            Criteria = p => p.Id == getUserByIdQuery.Id;
        }
    }
}
