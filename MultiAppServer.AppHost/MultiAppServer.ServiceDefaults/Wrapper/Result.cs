
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAppServer.ServiceDefaults.Wrapper
{
    public class ResultBase<T> : IResultBase<T>
    {
        public ResultBase()
        {
            StatusCode = 200;
            ErrorMessages = new List<string>();
            Success = true;

        }

        public ResultBase(T result)
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

        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public T Result { get; set; }
        public List<string> ErrorMessages { get; set; }

    }
}
