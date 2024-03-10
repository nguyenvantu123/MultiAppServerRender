using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorWeb.Wrapper
{
    public class ResultBase<T> : IResultBase<T>
    {
        public ResultBase()
        {
            StatusCode = 200;
            ErrorMessages = new List<string>();
        }

        public ResultBase(T result)
        {
            Success = true;
            Result = result;
            ErrorMessages = new List<string>();
            StatusCode = 200;
        }

        public ResultBase(int statusCode, params string[] errors)
        {
            Success = false;
            ErrorMessages = errors.ToList();
            StatusCode = statusCode;
        }

        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public T Result { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}