using BlazorIdentityApi.Users.Constants;
using BlazorIdentityApi.Dtos.Dashboard;
using BlazorIdentityApi.ExceptionHandling;
using BlazorIdentityApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorIdentityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json")]
    //[Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        //private readonly IDashboardIdentityService _dashboardIdentityService;

        public DashboardController(IDashboardService dashboardService
            //, IDashboardIdentityService dashboardIdentityService
            )
        {
            _dashboardService = dashboardService;
            //_dashboardIdentityService = dashboardIdentityService;
        }

        [HttpGet(nameof(GetDashboardIdentityServer))]
        public async Task<ActionResult<DashboardDto>> GetDashboardIdentityServer(int auditLogsLastNumberOfDays = 7)
        {
            var dashboardIdentityServer = await _dashboardService.GetDashboardIdentityServerAsync(auditLogsLastNumberOfDays);

            return Ok(dashboardIdentityServer);
        }

        //[HttpGet(nameof(GetDashboardIdentity))]
        //public async Task<ActionResult<DashboardIdentityDto>> GetDashboardIdentity()
        //{
        //    var dashboardIdentity = await _dashboardIdentityService.GetIdentityDashboardAsync();

        //    return Ok(dashboardIdentity);
        //}
    }
}