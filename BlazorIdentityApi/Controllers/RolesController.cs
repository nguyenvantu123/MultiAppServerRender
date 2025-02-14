using BlazorIdentityApi.Users.Constants;
using BlazorIdentityApi.Dtos.Identity;
using BlazorIdentityApi.Dtos.Roles;
using BlazorIdentityApi.ExceptionHandling;
using BlazorIdentityApi.Resources;
using BlazorIdentityApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BlazorIdentityApi.Models;

namespace BlazorIdentityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json", "application/problem+json")]
    //[Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public class RolesController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        private readonly IMapper _mapper;
        private readonly IApiErrorResources _errorResources;

        public RolesController(IMapper mapper, IApiErrorResources errorResources, IIdentityService identityService)
        {
            _mapper = mapper;
            _errorResources = errorResources;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> Get(Guid id)
        {
            var role = await _identityService.GetRoleAsync(id.ToString());

            return Ok(role);
        }

        [HttpGet]
        public async Task<ActionResult<RoleDto>> Get(string searchText, int page = 1, int pageSize = 10)
        {
            var rolesDto = await _identityService.GetRolesAsync(searchText, page, pageSize);

            return Ok(rolesDto);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<RoleDto>> Post([FromBody] RoleDto<Guid> role)
        {
            if (!EqualityComparer<Guid>.Default.Equals(role.Id, default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            var (identityResult, roleId) = await _identityService.CreateRoleAsync(role);
            var createdRole = await _identityService.GetRoleAsync(roleId.ToString());

            return CreatedAtAction(nameof(Get), new { id = createdRole.Id }, createdRole);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] RoleDto<Guid> role)
        {
            await _identityService.GetRoleAsync(role.Id.ToString());
            await _identityService.UpdateRoleAsync(role);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var roleDto = new RoleDto<Guid> { Id = id };

            await _identityService.GetRoleAsync(id.ToString());
            await _identityService.DeleteRoleAsync(roleDto);

            return Ok();
        }

        [HttpGet("{id}/Users")]
        public async Task<ActionResult<UserDto<Guid>>> GetRoleUsers(string id, string searchText, int page = 1, int pageSize = 10)
        {
            var usersDto = await _identityService.GetRoleUsersAsync(id, searchText, page, pageSize);

            return Ok(usersDto);
        }

        [HttpGet("{id}/Claims")]
        public async Task<ActionResult<RoleClaimsApiDtoList<Guid>>> GetRoleClaims(string id, int page = 1, int pageSize = 10)
        {
            var roleClaimsDto = await _identityService.GetRoleClaimsAsync(id, page, pageSize);
            var roleClaimsApiDto = _mapper.Map<RoleClaimsApiDtoList<Guid>>(roleClaimsDto);

            return Ok(roleClaimsApiDto);
        }

        [HttpPost("Claims")]
        public async Task<IActionResult> PostRoleClaims([FromBody] RoleClaimDto<Guid> roleClaims)
        {
            var roleClaimsDto = _mapper.Map<RoleClaimDto<Guid>>(roleClaims);

            if (!roleClaimsDto.ClaimId.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            await _identityService.CreateRoleClaimsAsync(roleClaimsDto);

            return Ok();
        }

        [HttpPut("Claims")]
        public async Task<IActionResult> PutRoleClaims([FromBody] RoleClaimDto<Guid> roleClaims)
        {
            var roleClaimsDto = _mapper.Map<RoleClaimDto<Guid>>(roleClaims);

            if (!roleClaimsDto.ClaimId.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            await _identityService.UpdateRoleClaimsAsync(roleClaimsDto);

            return Ok();
        }

        [HttpDelete("{id}/Claims")]
        public async Task<IActionResult> DeleteRoleClaims(Guid id, int claimId)
        {
            var roleDto = new RoleClaimDto<Guid> { ClaimId = claimId, RoleId = id };

            await _identityService.GetRoleClaimAsync(roleDto.RoleId.ToString(), roleDto.ClaimId);
            await _identityService.DeleteRoleClaimAsync(roleDto);

            return Ok();
        }
    }
}

