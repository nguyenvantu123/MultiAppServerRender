using System.Threading.Tasks;
using Toolbelt.Blazor;

namespace BlazorWeb.Infrastructure.Managers.Interceptors
{
    public interface IHttpInterceptorManager
    {
        void RegisterEvent();

        Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

        void DisposeEvent();
    }
}