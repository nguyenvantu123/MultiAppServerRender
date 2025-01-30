using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MultiAppServer.ServiceDefaults;

namespace BlazorApiUser.Filter
{
    public class MyActionFilterAttribute : IActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ModelState.IsValid)
            {
                var result = new OkObjectResult(new ApiResponseDto<int>(400,
                   string.Join(";", filterContext.ModelState.Values
                        .SelectMany(c => c.Errors)
                        .Select(c => c.ErrorMessage)
                        .ToArray())));
                filterContext.Result = result;
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}
