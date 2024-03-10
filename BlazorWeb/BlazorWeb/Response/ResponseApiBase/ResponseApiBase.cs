using BlazorWeb.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiAppServer.ServiceDefaults.Wrapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace BlazorWeb.Response.ResponseApiBase
{
    public class ResponseApiBase : IResponseApiBase
    {
        private readonly IAuthenticationManager _authenticationManager;
        public ResponseApiBase(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        public async Task<ResultBase<T>> CreateApiResponse<T>(HttpResponseMessage serviceResults)
        {
            Console.WriteLine($"CreateApiResponse {serviceResults.IsSuccessStatusCode}");

            var data = await serviceResults.Content.ReadAsStringAsync();

            var responseReturn = new ResultBase<T>();

            if (!serviceResults.IsSuccessStatusCode)
            {
                var responseCheck = JsonConvert.DeserializeObject<ResultBase<bool>>(await serviceResults.Content.ReadAsStringAsync());

                if (responseCheck.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    var token = await _authenticationManager.RefreshToken();

                    if (!string.IsNullOrEmpty(token))
                    {
                        responseReturn = new ResultBase<T>
                        {
                            StatusCode = 200,
                            Success = false,
                            ErrorMessages = responseCheck.ErrorMessages
                        };
                    }
                    else
                    {

                        await _authenticationManager.Logout();

                        responseReturn = new ResultBase<T>
                        {
                            StatusCode = 401,
                            Success = false,
                            ErrorMessages = responseCheck.ErrorMessages
                        };
                    }
                }
                else if (responseCheck.StatusCode != (int)HttpStatusCode.OK)
                {
                    responseReturn = new ResultBase<T>
                    {
                        StatusCode = responseCheck.StatusCode,
                        Success = false,
                        ErrorMessages = responseCheck.ErrorMessages
                    };
                }
            }
            else
            {
                responseReturn = JsonConvert.DeserializeObject<ResultBase<T>>(await serviceResults.Content.ReadAsStringAsync()); ;
            }

            return responseReturn;
        }

        //public async Task<ResultBase<T>> CreateApiResponse(HttpResponseMessage serviceResults)
        //{
        //    Console.WriteLine($"CreateApiResponse {serviceResults.IsSuccessStatusCode}");

        //    var response = JsonConvert.DeserializeObject<ResultBase<TViewModel>>(await serviceResults.Content.ReadAsStringAsync());

        //    if (response.StatusCode == (int)HttpStatusCode.Unauthorized &&
        //        response.ErrorMessages[0] == "The Token is expired.")
        //    {
        //        var token = await _authenticationManager.RefreshToken();

        //        if (!string.IsNullOrEmpty(token))
        //        {
        //            response = new ResultBase<TViewModel>
        //            {
        //                StatusCode = 200,
        //                Success = true,
        //                ErrorMessages = response.ErrorMessages
        //            };
        //        }
        //        else
        //        {

        //            await _authenticationManager.Logout();

        //            response = new ResultBase<TViewModel>
        //            {
        //                StatusCode = 401,
        //                Success = false,
        //                ErrorMessages = response.ErrorMessages
        //            };
        //        }
        //    }
        //    else if (response.StatusCode != (int)HttpStatusCode.OK)
        //    {
        //        response = new ResultBase<TViewModel>
        //        {
        //            StatusCode = response.StatusCode,
        //            Success = false,
        //            ErrorMessages = response.ErrorMessages
        //        };
        //    }

        //    return response;

        //}
    }
}
