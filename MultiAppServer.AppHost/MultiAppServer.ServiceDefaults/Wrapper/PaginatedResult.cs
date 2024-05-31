
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAppServer.ServiceDefaults.Wrapper
{
    public class PaginatedResult<T> : IResultBase
    {
        public PaginatedResult(List<T>? result)
        {
            Result = result;
        }
        
        public List<T>? Result { get; set; }
        private PaginatedResult(bool succeeded, List<T>? result = default, List<string>? messages = null, int count = 0, int page = 1, int pageSize = 10)
        {
            Result = result;
            CurrentPage = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Success = succeeded;
        }

        public static PaginatedResult<T> Failure(List<string>? messages)
        {
            return new PaginatedResult<T>(false, default, messages);
        }

        public new static PaginatedResult<T> Succeeded(List<T>? data, int count, int page, int pageSize)
        {
            return new PaginatedResult<T>(true, data, null, count, page, pageSize);
        }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}