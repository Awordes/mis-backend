using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Common.Extensions
{
    public static class PagedResultExtension
    {
        public static async Task<PagedResult<TResult>> GetPagedAsync<TSource, TResult>
            (this IQueryable<TSource> query, int page, int pageSize, IMapper mapper, CancellationToken cancellationToken)
            where TResult : class
        {
            var result = new PagedResult<TResult>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            
            var source =  await query.Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            result.Results = mapper.Map<ICollection<TResult>>(source);

            return result;
        }
    }
}