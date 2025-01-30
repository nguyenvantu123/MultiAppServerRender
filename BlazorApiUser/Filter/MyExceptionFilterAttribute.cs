using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MultiAppServer.ServiceDefaults;

namespace BlazorApiUser.Filter
{
    public class MyExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// The function for catching action exception
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            var message = context.Exception.InnerException != null
                ? context.Exception.InnerException.Message
                : context.Exception.Message;
            context.Result = new OkObjectResult(new ApiResponseDto<bool>(500, message));
        }
    }
}
