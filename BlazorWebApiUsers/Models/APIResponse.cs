using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BlazorWebApi.Users.Models
{
    [Serializable]
    [DataContract]
    public class ApiResponse<T>
    {
        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public int StatusCode { get; set; }

        public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode < 300;

        [DataMember]
        public string Message { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public T Result { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Count { get; set; } = 0;

        [JsonConstructor]
        public ApiResponse(int statusCode, string message = "", T result = default, int count = 0)
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
            Count = count;
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            if (string.IsNullOrWhiteSpace(message))
                Message = IsSuccessStatusCode ? "Operation Successful" : "Operation Failed";
        }

        static public implicit operator ApiResponse<T>(ApiResponse r)
        {
            return new ApiResponse<T>(r.StatusCode, r.Message, r.Result is T ? (T)r.Result : default);
        }
    }

    [Serializable]
    [DataContract]
    public class ApiResponse : ApiResponse<object>
    {
        [JsonConstructor]
        public ApiResponse(int statusCode, string message = "", object result = null, int count = 0) : base(statusCode, message)
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
            Count = count;
        }

        public ApiResponse(int statusCode) : base(statusCode, "")
        {
            StatusCode = statusCode;
        }
    }
}
