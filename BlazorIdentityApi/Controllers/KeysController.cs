using BlazorIdentityApi.Dtos.Key;
using BlazorIdentityApi.ExceptionHandling;
using BlazorIdentityApi.Services.Interfaces;
using BlazorIdentityApi.UI.Api.Mappers;
using BlazorIdentityApi.Users.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorIdentityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json", "application/problem+json")]
    //[Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public class KeysController : ControllerBase
    {
        private readonly IKeyService _keyService;

        public KeysController(IKeyService keyService)
        {
            _keyService = keyService;
        }

        [HttpGet]
        public async Task<ActionResult<KeysApiDto>> Get(int page = 1, int pageSize = 10)
        {
            var keys = await _keyService.GetKeysAsync(page, pageSize);
            var keysApi = keys.ToKeyApiModel<KeysApiDto>();

            return Ok(keysApi);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KeyApiDto>> Get(string id)
        {
            var key = await _keyService.GetKeyAsync(id);

            var keyApi = key.ToKeyApiModel<KeyApiDto>();

            return Ok(keyApi);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _keyService.DeleteKeyAsync(id);

            return Ok();
        }
    }
}
