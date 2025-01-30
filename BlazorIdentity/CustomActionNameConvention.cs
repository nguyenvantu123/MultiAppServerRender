using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BlazorIdentity.Users
{
    public class CustomActionNameConvention : IActionModelConvention
    {
        private readonly IOutboundParameterTransformer _parameterTransformer;

        public CustomActionNameConvention(IOutboundParameterTransformer parameterTransformer)
        {
            _parameterTransformer = parameterTransformer;
        }

        public void Apply(ActionModel action)
        {
            var actionName = action.ActionName;
            var transformedActionName = _parameterTransformer.TransformOutbound(actionName);
            foreach (var selector in action.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel.Template =
                        selector.AttributeRouteModel.Template.Replace("[action]", transformedActionName);
                }
            }
        }
    }
}