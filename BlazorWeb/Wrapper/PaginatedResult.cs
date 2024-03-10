<<<<<<<< HEAD:BlazorWeb/Wrapper/PaginatedResult.cs
﻿using Wrapper;
using System;
========
﻿using System;
>>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b:MultiAppServer.AppHost/MultiAppServer.ServiceDefaults/Wrapper/PaginatedResult.cs
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

<<<<<<<< HEAD:BlazorWeb/Wrapper/PaginatedResult.cs
namespace Wrapper
========
namespace MultiAppServer.ServiceDefaults.Wrapper
>>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b:MultiAppServer.AppHost/MultiAppServer.ServiceDefaults/Wrapper/PaginatedResult.cs
{
    public class PaginatedResult<T> : ResultBase<T>
    {
        public PaginatedResult(List<T> data)
        {
            Data = data;
        }

        public List<T> Data { get; set; }

        internal PaginatedResult(bool succeeded, List<T> data = default, List<string> messages = null, int count = 0, int page = 1, int pageSize = 10)
        {
            Data = data;
            CurrentPage = page;
            succeeded = succeeded;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
        }

        public static PaginatedResult<T> Failure(List<string> messages)
        {
            return new PaginatedResult<T>(false, default, messages);
        }

        public static PaginatedResult<T> Success(List<T> data, int count, int page, int pageSize)
        {
            return new PaginatedResult<T>(true, data, null, count, page, pageSize);
        }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
        public int PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
    }
}