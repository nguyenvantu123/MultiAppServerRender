using BlazorWebApi.Users.Exceptions;
using BlazorWebApi.Users.Specifications.Base;
using Microsoft.EntityFrameworkCore;
using ServiceDefaults;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using BlazorWeb.Contracts;
//using MultiAppServer.ServiceDefaults.Wrapper;

namespace BlazorWebApi.Users.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize) where T : class
        {
            if (source == null) throw new ApiException();
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;
            int count = await source.CountAsync();
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return PaginatedResult<T>.Succeeded(items, count, pageNumber, pageSize);
        }

        public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> spec) where T : class
        {
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(query,
                    (current, include) => current.Include(include));
            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));
            if (spec.Criteria== null)
            {
                return secondaryResult;
            }
            return secondaryResult.Where(spec.Criteria);
        }
    }
}