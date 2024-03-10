using Wrapper;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
<<<<<<< HEAD:BlazorWeb/Extensions/ResultExtensions.cs
=======
//using IResult = Wrapper.IResult;
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b:BlazorWeb/BlazorWeb/Extensions/ResultExtensions.cs

namespace BlazorWeb.Extensions
{
    public static class ResultExtensions
    {
        public static async Task<IResultBase<T>> ToResult<T>(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
<<<<<<< HEAD:BlazorWeb/Extensions/ResultExtensions.cs
            var responseObject = JsonSerializer.Deserialize<IResultBase<T>>(responseAsString, new JsonSerializerOptions
=======
            var responseObject = JsonSerializer.Deserialize<ResultBase<T>>(responseAsString, new JsonSerializerOptions
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b:BlazorWeb/BlazorWeb/Extensions/ResultExtensions.cs
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject;
        }

<<<<<<< HEAD:BlazorWeb/Extensions/ResultExtensions.cs
        public static async Task<ResultBase> ToResult(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ResultBase>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject;
        }
=======
        //public static async Task<IResultBase> ToResult(this HttpResponseMessage response)
        //{
        //    var responseAsString = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonSerializer.Deserialize<IResult>(responseAsString, new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true,
        //        ReferenceHandler = ReferenceHandler.Preserve
        //    });
        //    return responseObject;
        //}
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b:BlazorWeb/BlazorWeb/Extensions/ResultExtensions.cs

        public static async Task<PaginatedResult<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return responseObject;
        }
    }
}