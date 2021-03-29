using System;

namespace Core.Application.Common.Pagination
{
    /// <summary>
    /// Базовый класс для страничного результата
    /// </summary>
    public abstract class PagedResultBase
    {
        /// <summary>
        /// Номер текущей страницы
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Общее кол-во страниц
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Кол-во элементов на странице
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Общее кол-во элементов
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// Номер первого элемента на странице
        /// </summary>
        public int FirstRowOnPage => (CurrentPage - 1) * PageSize + 1;

        /// <summary>
        /// Номер последнего элемента на странице
        /// </summary>
        public int LastRowOnPage => Math.Min(CurrentPage * PageSize, RowCount);        
    }
}