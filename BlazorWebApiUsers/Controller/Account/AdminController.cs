using AutoMapper;
using BlazorWebApi.Repositories;
using BlazorWebApi.Users.Constants;
using BlazorWebApi.Users.Data;
using BlazorWebApi.Users.Extensions;
using BlazorWebApi.Users.Models;
using BlazorWebApi.Users.Request;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Net;
using WebApp.Models;
using static BlazorWebApi.Users.Models.Permissions;

namespace BlazorWebApi.Users.Controller.Account
{
    [OpenApiIgnore]
    [SecurityHeaders]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {

        private readonly IMapper _autoMapper;
        private readonly TenantStoreDbContext _tenantStoreDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EntityPermissions _entityPermissions;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AdminController> _logger;

        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _accessor;
        private readonly IServiceProvider _serviceProvider;

        private readonly RedisUserRepository _redisUserRepository;

        protected readonly IHttpContextAccessor httpContextAccessor;


        public AdminController(IMapper autoMapper, TenantStoreDbContext tenantStoreDbContext, UserManager<ApplicationUser> userManager, EntityPermissions entityPermissions, RoleManager<ApplicationRole> roleManager, ILogger<AdminController> logger, ApplicationDbContext dbContext, IHttpContextAccessor accessor, IServiceProvider serviceProvider, RedisUserRepository redisUserRepository)
        {
            _autoMapper = autoMapper;
            _tenantStoreDbContext = tenantStoreDbContext;
            _userManager = userManager;
            _entityPermissions = entityPermissions;
            _roleManager = roleManager;
            _logger = logger;

            _dbContext = dbContext;
            _accessor = accessor;
            _serviceProvider = serviceProvider;

            _redisUserRepository = redisUserRepository;
        }


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public ApiResponse Tenant()
            => new ApiResponse((int)HttpStatusCode.OK, string.Empty, _autoMapper.Map<TenantModel>(HttpContext.GetMultiTenantContext<AppTenantInfo>().TenantInfo));

        #region Tenants
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

            return new ApiResponse(200, $"{count} tenants fetched", await _autoMapper.ProjectTo<TenantModel>(query).ToListAsync(), count);
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
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
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
        #endregion

        #region Users
        [HttpGet]
        [Route("[action]")]
        [Authorize(Permissions.User.Read)]
        public async Task<ApiResponseDto<List<UserViewModel>>> Users([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 0)
        {
            var userList = _userManager.Users.Include(x => x.UserRoles).AsQueryable();
            var count = userList.Count();
            var listUsers = userList.OrderBy(x => x.Id).Skip(pageNumber * pageSize).Take(pageSize).ToList();

            var userDtoList = _autoMapper.Map<List<UserViewModel>>(listUsers);

            return new ApiResponseDto<List<UserViewModel>>(200, $"{count} users fetched", userDtoList, count);
        }

        [HttpGet]
        [Authorize(Permissions.User.Read)]
        [Route("[action]/{id}")]
        public async Task<ApiResponseDto<UserViewModel>> UserById([FromQuery] string id)
        {
            var userById = await _userManager.FindByIdAsync(id);

            if (userById == null)
            {
                return new ApiResponseDto<UserViewModel>(404, "User Not Found", null);
            }

            var result = _autoMapper.Map<UserViewModel>(userById);

            return new ApiResponseDto<UserViewModel>(200, "Success", result);
        }


        [HttpGet]
        [Authorize(Permissions.User.Read)]
        [Route("[action]/{id}")]
        public async Task<ApiResponseDto<UserRolesResponse>> UserRoles([FromQuery] string id)
        {
            var viewModel = new List<UserRoleModel>();
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRoleModel
                {
                    RoleName = role.Name,
                    RoleDescription = role.Description
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                viewModel.Add(userRolesViewModel);
            }
            var result = new UserRolesResponse { UserRoles = viewModel };
            return new ApiResponseDto<UserRolesResponse>(200, "Success", result);
        }

        [HttpPut]
        [Authorize(Permissions.User.Update)]
        [Route("[action]")]
        public async Task<ApiResponseDto> UserRoles(UpdateUserRolesRequest request)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(request.UserId);
            if (user.UserName == DefaultUserNames.Administrator)
            {
                return new ApiResponseDto(400, "Not Allowed");
            }

            var userAccessor = httpContextAccessor.HttpContext.User;
            var userId = new Guid(userAccessor.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var roles = await _userManager.GetRolesAsync(user);
            var selectedRoles = request.UserRoles.Where(x => x.Selected).ToList();

            var currentUser = await _userManager.FindByIdAsync(userId.ToString());
            if (!await _userManager.IsInRoleAsync(currentUser, DefaultRoleNames.Administrator))
            {
                var tryToAddAdministratorRole = selectedRoles
                    .Any(x => x.RoleName == DefaultRoleNames.Administrator);
                var userHasAdministratorRole = roles.Any(x => x == DefaultRoleNames.Administrator);
                if (tryToAddAdministratorRole && !userHasAdministratorRole || !tryToAddAdministratorRole && userHasAdministratorRole)
                {

                    return new ApiResponseDto(400, "Not Allowed to add or delete Administrator Role if you have not this role.");
                    //return await Result.FailAsync(_localizer["Not Allowed to add or delete Administrator Role if you have not this role."]);
                }
            }

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            result = await _userManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));

            return new ApiResponseDto(200, "Roles Updated");
            //return await Result.SuccessAsync(_localizer["Roles Updated"]);
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize(Permissions.User.Update)]
        public async Task<ApiResponseDto> ToggleUserStatus([FromBody] ToggleUserStatusRequest toggleUserStatusRequest)
        {
            var userById = await _userManager.FindByIdAsync(toggleUserStatusRequest.UserId);

            if (userById == null)
            {
                return new ApiResponseDto(404, "User Not Found", null);
            }

            userById.IsActive = toggleUserStatusRequest.ActivateUser;

            await _userManager.UpdateAsync(userById);

            return new ApiResponseDto(200, "Success", null);
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public ApiResponse GetPermissions()
        {
            var permissions = _entityPermissions.GetAllPermissionNames();
            return new ApiResponse(200, "Permissions list fetched", permissions);
        }
        #endregion

        #region Roles
        [HttpGet]
        [Route("[action]")]
        [Authorize(Permissions.Role.Read)]
        public async Task<ApiResponse> GetRoles([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 0, [FromQuery] string search = "")
        {
            var roleQuery = _roleManager.Roles.AsQueryable().OrderBy(x => x.Name);
            var count = roleQuery.Count();
            var listResponse = (pageSize > 0 ? roleQuery.Skip(pageNumber * pageSize).Take(pageSize) : roleQuery).ToList();

            var roleDtoList = new List<RoleDto>();

            foreach (var role in listResponse)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                List<string> permissions = claims.OrderBy(x => x.Value).Where(x => x.Type == ApplicationClaimTypes.Permission).Select(x => _entityPermissions.GetPermissionByValue(x.Value).Name).ToList();

                roleDtoList.Add(new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Permissions = permissions
                });
            }

            return new ApiResponse(200, $"{count} roles fetched", roleDtoList, count);
        }

        [HttpGet]
        [Route("[action]/{name}")]
        [Authorize]
        public async Task<ApiResponse> GetRoleAsync(string name)
        {
            var identityRole = await _roleManager.FindByNameAsync(name);

            var claims = await _roleManager.GetClaimsAsync(identityRole);
            var permissions = claims.OrderBy(x => x.Value).Where(x => x.Type == ApplicationClaimTypes.Permission).Select(x => _entityPermissions.GetPermissionByValue(x.Value).Name).ToList();

            var roleDto = new RoleDto
            {
                Name = name,
                Permissions = permissions
            };

            return new ApiResponse(200, "Role fetched", roleDto);
        }


        [HttpPost]
        [Route("[action]")]
        [Authorize(Permissions.Role.Create)]
        public async Task<ApiResponse> CreateRoleAsync([FromBody] RoleDto roleDto)
        {
            if (_roleManager.Roles.Any(r => r.Name == roleDto.Name))
                return new ApiResponse(400, $"Role {roleDto.Name} already exists");

            var result = await _roleManager.CreateAsync(new ApplicationRole(roleDto.Name));

            if (!result.Succeeded)
            {
                var msg = result.GetErrors();
                _logger.LogWarning($"Error while creating role {roleDto.Name}: {msg}");
                return new ApiResponse(400, msg);
            }

            // Re-create the permissions
            var role = await _roleManager.FindByNameAsync(roleDto.Name);

            foreach (var claim in roleDto.Permissions)
            {
                var resultAddClaim = await _roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, _entityPermissions.GetPermissionByName(claim)));

                if (!resultAddClaim.Succeeded)
                    await _roleManager.DeleteAsync(role);
            }

            return new ApiResponse(200, $"Role {roleDto.Name} created", roleDto); //fix a strange System.Text.Json exception shown only in Debug_SSB
        }


        [HttpPut]
        [Route("[action]")]
        [Authorize(Permissions.Role.Update)]
        public async Task<ApiResponse> UpdateRoleAsync([FromBody] RoleDto roleDto)
        {
            var response = new ApiResponse(200, $"Role {roleDto.Name} updated", roleDto);

            if (!_roleManager.Roles.Any(r => r.Name == roleDto.Name))
                response = new ApiResponse(400, $"The role {roleDto.Name} doesn't exist");
            else
            {
                if (roleDto.Name == DefaultRoleNames.Administrator)
                    response = new ApiResponse(403, $"Role {roleDto.Name} cannot be edited");
                else
                {
                    // Create the permissions
                    var role = await _roleManager.FindByNameAsync(roleDto.Name);

                    var claims = await _roleManager.GetClaimsAsync(role);
                    var permissions = claims.OrderBy(x => x.Value).Where(x => x.Type == ApplicationClaimTypes.Permission).Select(x => x.Value).ToList();

                    foreach (var permission in permissions)
                    {
                        await _roleManager.RemoveClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, permission));
                    }

                    foreach (var claim in roleDto.Permissions)
                    {
                        var result = await _roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, _entityPermissions.GetPermissionByName(claim)));

                        if (!result.Succeeded)
                            await _roleManager.DeleteAsync(role);
                    }
                }
            }

            return response;
        }


        [HttpDelete]
        [Route("[action]/{name}")]
        [Authorize(Permissions.Role.Delete)]
        public async Task<ApiResponse> DeleteRoleAsync(string name)
        {
            var response = new ApiResponse(200, $"Role {name} deleted", name);

            // Check if the role is used by a user
            var users = await _userManager.GetUsersInRoleAsync(name);
            if (users.Any())
                response = new ApiResponse(404, name);
            else
            {
                if (name == DefaultRoleNames.Administrator)
                    response = new ApiResponse(403, $"Role {name} cannot be deleted", name);
                else
                {
                    // Delete the role
                    var role = await _roleManager.FindByNameAsync(name);
                    await _roleManager.DeleteAsync(role);
                }
            }

            return response;
        }


        [HttpDelete]
        [Route("[action]/{name}")]
        [Authorize(Permissions.Role.Delete)]
        public async Task<ApiResponse> GetRolesAsync(string name)
        {
            var response = new ApiResponse(200, $"Role {name} deleted", name);

            // Check if the role is used by a user
            var users = await _userManager.GetUsersInRoleAsync(name);
            if (users.Any())
                response = new ApiResponse(404, name);
            else
            {
                if (name == DefaultRoleNames.Administrator)
                    response = new ApiResponse(403, $"Role {name} cannot be deleted", name);
                else
                {
                    // Delete the role
                    var role = await _roleManager.FindByNameAsync(name);
                    await _roleManager.DeleteAsync(role);
                }
            }

            return response;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<UserProfile> UserProfile()
        {

            var user = _accessor.HttpContext.User;

            var userProfileResult = new UserProfile();
            var userId = new Guid(user.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
            userProfileResult = await _redisUserRepository.GetUserProfileAsync(userId);

            if (userProfileResult == null)
            {
                userProfileResult = await _dbContext.UserProfiles.SingleOrDefaultAsync(i => i.ApplicationUser.NormalizedUserName == user.Identity.Name.ToUpper());

                if (userProfileResult == null)
                {
                    var tenantId = _accessor.HttpContext.GetMultiTenantContext<AppTenantInfo>().TenantInfo.Id;


                    userProfileResult = await _dbContext.UserProfiles.SingleOrDefaultAsync(i => i.TenantId == tenantId && i.UserId == userId);

                    if (userProfileResult == null)
                    {
                        userProfileResult = new UserProfile { TenantId = tenantId, UserId = userId };

                        _dbContext.UserProfiles.Add(userProfileResult);
                    }

                    userProfileResult.LastUpdatedDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();
                }

                await _redisUserRepository.UpdateUserProfileAsync(userProfileResult);
            }


            return userProfileResult;
        }
        #endregion
    }
}