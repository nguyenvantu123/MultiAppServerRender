using MultiAppServer.ServiceDefaults.Wrapper;

namespace BlazorWeb.Response.ResponseApiBase
{
    public interface IResponseApiBase
    {

        Task<ResultBase<TView>> CreateApiResponse<TView>(HttpResponseMessage serviceResults);
    }
}
