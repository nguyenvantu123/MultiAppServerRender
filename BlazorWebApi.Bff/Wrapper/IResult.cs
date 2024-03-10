using System.Collections.Generic;

namespace BlazorWebApi.Bff.Wrapper
{
    public interface IResultBase<T>
    {
        int StatusCode { get; set; }
        bool Success { get; set; }
        List<string> ErrorMessages { get; set; }
        T Result { get; set; }
    }
}