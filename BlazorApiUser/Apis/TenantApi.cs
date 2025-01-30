using AutoMapper;
using BlazorApiUser.Commands.Users;
using BlazorApiUser.Queries.Roles;
using BlazorApiUser.Queries.Users;
using BlazorIdentity.Models;
using BlazorIdentity.Users.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using System.Security.Claims;
using WebApp.Models;

namespace BlazorApiUser.Apis
{
    public static class TenantApi
    {
        public static RouteGroupBuilder MapTenantsApiV1(this IEndpointRouteBuilder app)
        {
            var api = app.MapGroup("api/admin").HasApiVersion(1.0);

            api.MapGet("/tenants", GetTenants);

            api.MapGet("/tenant-by-user", GetTenantByUser);

            //api.MapPost("/users/create", Create);

            //api.MapPut("/users/update/{id}", AdminUpdateUser);

            //api.MapDelete("/users/{id}", AdminDelete);

            //api.MapPost("/users/reset-password", ResetPasswordUser);

            //api.MapGet("/users/get-permission-by-user", GetPermissionByUser);

            //api.MapGet("/users/{id}", GetUserById);

            //api.MapGet("/users/user-roles/{id}", GetRoleByUserId);

            //api.MapPut("/users/update-user-roles", UpdateUserRoles);

            //api.MapPut("/users/toggle-user-status", ToggleUserStatus);

            //api.MapGet("/users/user-view-model", UserViewModel);

            return api;
        }

        [Authorize(Roles = Permissions.Tenant.Read)]
        public static async Task<ApiResponse<List<TenantDto>>> GetTenants([AsParameters] GetListRoleQuery getListRoleQuery, [AsParameters] UserServices userServices)
        {

            //var userLst = await userServices.Mediator.Send(getListRoleQuery);

            var query = await Filter(getListRoleQuery, userServices);

            var totalItems = await query.CountAsync();

            var itemsOnPage = await query
                .Skip(getListRoleQuery.pageSize * getListRoleQuery.pageNumber)
                .Take(getListRoleQuery.pageSize)
                .ToListAsync();

            List<TenantDto> tenantDto = userServices.Mapper.Map<List<TenantDto>>(itemsOnPage);

            return new ApiResponse<List<TenantDto>>(200, $"{totalItems} users fetched", tenantDto, totalItems);
        }

        [Authorize]
        public static async Task<ApiResponse<TenantDto>> GetTenantByUser(ClaimsPrincipal userAuth, [AsParameters] UserServices userServices)
        {

            var user = await userServices.UserManager.FindByIdAsync(userAuth!.FindFirstValue("sub")!);

            if (user == null)
            {
                return new ApiResponse(400, $"User doesn't exist!!!!");
            }

            if (string.IsNullOrEmpty(user.TenantId))
            {
                return new ApiResponse(200, "", new TenantDto { Id = "", Name = "" });
            }

            var dto = userServices.TenantStoreDbContext.AppTenantInfo.Where(x => x.Id == user.TenantId).FirstOrDefault();

            if (dto == null)
            {
                return new ApiResponse(200, "", new TenantDto { Id = "", Name = "" });
            }

            TenantDto tenantDto = userServices.Mapper.Map<TenantDto>(dto);

            return new ApiResponse<TenantDto>(200, "", tenantDto);
        }

        private static async Task<DbSet<AppTenantInfo>> Filter(GetListRoleQuery SearchQuery, UserServices userServices)
        {
            var queryable = userServices.TenantStoreDbContext.AppTenantInfo;

            if (!string.IsNullOrEmpty(SearchQuery.name))
            {
                queryable.Where(p => p.Name.Contains(SearchQuery.name));
            }

            return queryable;
        }
    }

}
