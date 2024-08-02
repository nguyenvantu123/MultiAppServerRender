using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAppServer.ServiceDefaults
{
    public class GuidHeaderModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(Guid)) return Task.CompletedTask;
            if (!bindingContext.BindingSource.CanAcceptDataFrom(BindingSource.Header)) return Task.CompletedTask;

            var headerName = bindingContext.ModelName;
            var stringValue = bindingContext.HttpContext.Request.Headers[headerName];
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, stringValue, stringValue);

            // Attempt to parse the guid                
            if (Guid.TryParse(stringValue, out var valueAsGuid))
            {
                bindingContext.Result = ModelBindingResult.Success(valueAsGuid);
            }

            return Task.CompletedTask;
        }
    }
}