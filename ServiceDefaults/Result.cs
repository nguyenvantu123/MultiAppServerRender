
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDefaults
{
    [DataContract]
    public class ApiResponseDto<T> 
    {
        public ApiResponseDto()
        {
            StatusCode = 200;
            ErrorMessages = new List<string>();
            Success = true;

        }

     
        public ApiResponseDto(T result)
        {
            Success = true;
            Result = result;
            ErrorMessages = new List<string>();
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
        public List<string> ErrorMessages { get; set; }

    }


    [DataContract]
    public class ApiResponseDto : ApiResponseDto<object>
    {
    }
}
