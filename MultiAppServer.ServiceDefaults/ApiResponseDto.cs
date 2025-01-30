
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MultiAppServer.ServiceDefaults
{
    public class ApiResponseDto<T>
    {

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public int StatusCode { get; set; }

        [DataMember]
        public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode < 300;

        [DataMember]
        public string Message { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public T Result { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Count { get; set; } = 0;

        [JsonConstructor]
        public ApiResponseDto(int statusCode, string message = "", T result = default, int count = 0)
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
            Count = count;
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            if (string.IsNullOrWhiteSpace(message))
                Message = IsSuccessStatusCode ? "Operation Successful" : "Operation Failed";
        }

        static public implicit operator ApiResponseDto<T>(ApiResponseDto r)
        {
            return new ApiResponseDto<T>(r.StatusCode, r.Message, r.Result is T ? (T)r.Result : default);
        }

    }


    [Serializable]
    [DataContract]
    public class ApiResponseDto : ApiResponseDto<object>
    {
        [JsonConstructor]
        public ApiResponseDto(int statusCode, string message = "", object result = null, int count = 0) : base(statusCode, message)
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
            Count = count;
        }

        public ApiResponseDto(int statusCode) : base(statusCode, "")
        {
            StatusCode = statusCode;
        }
    }
}
