using AutoMapper;
using BlazorWebApi.Users.Data;
using BlazorWebApi.Users.Models;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Net;
using WebApp.Models;

namespace BlazorWebApi.Users.Controller.Account
{
    [OpenApiIgnore]
    [SecurityHeaders]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IMapper _autoMapper;

        private readonly TenantStoreDbContext _tenantStoreDbContext;

        public AdminController(IMapper autoMapper, TenantStoreDbContext tenantStoreDbContext)
        {
            _autoMapper = autoMapper;
            _tenantStoreDbContext = tenantStoreDbContext;
        }


        [HttpGet]
        [Route("[action]")]
        public ApiResponse Tenant()
            => new ApiResponse((int)HttpStatusCode.OK, string.Empty, _autoMapper.Map<TenantModel>(HttpContext.GetMultiTenantContext<AppTenantInfo>().TenantInfo));

        //#region Tenants
        [HttpGet]
        [Route("[action]")]
        [Authorize(Permissions.Tenant.Read)]
        public async Task<ApiResponse> Tenants([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 0, [FromQuery] string search = "")
        {
            var query = _tenantStoreDbContext.AppTenantInfo.OrderBy(i => i.Id).AsQueryable();


            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }

            var count = query.Count();

            if (pageSize > 0)
                query = query.Skip(pageNumber * pageSize).Take(pageSize);

            return new ApiResponse(200, $"{count} tenants fetched", await _autoMapper.ProjectTo<TenantModel>(query).ToListAsync());
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize]
        public async Task<ApiResponse> Tenant(string id)
        {
            var tenant = await _tenantStoreDbContext.AppTenantInfo.SingleOrDefaultAsync(i => i.Id == id);

            return tenant != null ? new ApiResponse(200, "Retrieved tenant", _autoMapper.Map<TenantModel>(tenant)) :
                                    new ApiResponse(404, "Failed to Retrieve Tenant");
        }


        [HttpPost]
        [Route("[action]")]
        [Authorize(Permissions.Tenant.Create)]
        public async Task<ApiResponse> CreateTenantAsync([FromBody] TenantModel tenantDto)
        {

            var tenant = _autoMapper.Map<TenantModel, AppTenantInfo>(tenantDto);
            await _tenantStoreDbContext.AppTenantInfo.AddAsync(tenant);
            await _tenantStoreDbContext.SaveChangesAsync();

            return new ApiResponse(200, $"Tenant {tenantDto.Name} created", tenantDto);
        }


        [HttpPut]
        [Route("[action]")]
        [Authorize(Permissions.Tenant.Update)]
        public async Task<ApiResponse> UpdateTenantAsync([FromBody] TenantModel tenantDto)
        {
            var tenant = await _tenantStoreDbContext.AppTenantInfo.SingleOrDefaultAsync(i => i.Id == tenantDto.Id);

            if (tenant == null)
                return new ApiResponse(400, $"Tenant {tenantDto.Name} created");

            var response = new ApiResponse(200, $"Tenant {tenantDto.Name} created", tenantDto);

            if (tenantDto.Identifier != tenant.Identifier && (tenantDto.Identifier == Constants.DefaultTenant.DefaultTenantId || tenant.Identifier == Constants.DefaultTenant.DefaultTenantId))
                response = new ApiResponse(403, "Default Tenant identifier cannot be changed and must be unique");
            else
            {
                tenant = _autoMapper.Map(tenantDto, tenant);

                _tenantStoreDbContext.AppTenantInfo.Update(tenant);
                await _tenantStoreDbContext.SaveChangesAsync();
            }

            return response;
        }

        [HttpDelete]
        [Route("[action]/{id}")]
        [Authorize(Permissions.Tenant.Delete)]
        public async Task<ApiResponse> DeleteTenantAsync(string id)
        {
            var response = new ApiResponse(200, $"Tenant {id} created");

            if (id == Constants.DefaultTenant.DefaultTenantId)
                response = new ApiResponse(403, $"Tenant {id} cannot be deleted");
            else
            {
                var tenant = await _tenantStoreDbContext.AppTenantInfo.SingleOrDefaultAsync(i => i.Id == id);

                if (tenant == null)
                    return new ApiResponse(400, $"The tenant {id} doesn't exist");

                _tenantStoreDbContext.AppTenantInfo.Remove(tenant);
                await _tenantStoreDbContext.SaveChangesAsync();
            }

            return response;
        }
        //#endregion
    }
}