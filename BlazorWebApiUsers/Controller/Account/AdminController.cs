using AutoMapper;
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

        public AdminController(IMapper autoMapper)
        {
            _autoMapper = autoMapper;
        }


        [HttpGet]
        [Route("[action]")]
        public ApiResponse Tenant()
            => new ApiResponse((int)HttpStatusCode.OK, string.Empty, _autoMapper.Map<TenantModel>(HttpContext.GetMultiTenantContext<AppTenantInfo>().TenantInfo));
    }
}
