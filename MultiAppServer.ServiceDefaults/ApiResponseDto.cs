
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MultiAppServer.ServiceDefaults
{
    public class ApiResponseDto<T>
    {
        public ApiResponseDto()
        {
            StatusCode = 200;
            Success = true;

        }

        public ApiResponseDto(T result)
        {
            Success = true;
            Result = result;
            StatusCode = 200;
        }

        //public ResultBase(int statusCode, params string[] errors)
        //{
        //    Success = true;
        //    ErrorMessages = errors.ToList();
        //    StatusCode = statusCode;
        //}

        [DataMember(EmitDefaultValue = false)]
        public T Result { get; set; }

        [DataMember]
        public int StatusCode { get; set; }

        [DataMember]
        public bool Success { get; set; }

        [DataMember] 
        public string Message { get; set; }

    }

    [DataContract]
    public class ApiResponseDto : ApiResponseDto<object>
    {
    }
}
