using BlazorWebApi.Bff.Wrapper;
using Newtonsoft.Json;
<<<<<<< HEAD
=======
using Swashbuckle.Swagger;
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWebApi.Bff
{
    public class AuthenticationHeaderHandler : DelegatingHandler
    {
        private IHttpContextAccessor _httpContextAccessor;

        public AuthenticationHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {

            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await base.SendAsync(request, cancellationToken);

<<<<<<< HEAD
            //string data = await response.Content.ReadAsStringAsync();
=======
            string data = await response.Content.ReadAsStringAsync();
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b

            return response;

        }
    }
}