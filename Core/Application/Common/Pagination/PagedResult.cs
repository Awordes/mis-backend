using System.Collections.Generic;

namespace Core.Application.Common.Pagination
{
    public class PagedResult<T> : PagedResultBase where T : class
    {
        public PagedResult()
        {
            Results = new List<T>();
        }

        /// <summary>
        /// Список результатов
        /// </summary>
        public ICollection<T> Results { get; set; }
    }
}