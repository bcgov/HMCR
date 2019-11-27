using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Model.Dtos
{
    public interface IPagedDto<T>
    {
        int TotalCount { get; set; }

        int PageSize { get; set; }
        int PageCount { get; }

        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }

    public class PagedDto<T> : IPagedDto<T>
    {
        public static int DefaultPageSize => 10;

        public IEnumerable<T> SourceList { get; set; }

        private int _pageNumber = 1;
        public int PageNumber
        {
            get { return _pageNumber; }
            set
            {
                _pageNumber = (value <= 0 ? 1 : value);
            }
        }

        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int PageCount => PageSize == 0 ? 1 : ((int)(TotalCount / PageSize) + (TotalCount % PageSize == 0 ? 0 : 1));
        public bool HasPreviousPage => PageNumber != 1;
        public bool HasNextPage => PageNumber < PageCount;
    }
}
