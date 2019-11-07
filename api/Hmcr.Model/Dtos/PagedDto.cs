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

        private IEnumerable<T> _sourceList = null;
        private int _pageSize = 0;
        private int _pageNumber = 0;
        private string _orderBy;

        public PagedDto(IEnumerable<T> sourceList = null, int pageSize = 0, int pageNumber = 0, string orderBy = null)
        {
            _sourceList = sourceList;
            _pageSize = pageSize;
            _pageNumber = pageNumber;
            _orderBy = orderBy;
        }

        public IEnumerable<T> SourceList
        {
            get { return _sourceList; }
            set
            {
                _sourceList = value;
            }
        }

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
        public int PageCount => _pageSize == 0 ? 1 : ((int)(TotalCount / _pageSize) + (TotalCount % _pageSize == 0 ? 0 : 1));
        public bool HasPreviousPage => _pageNumber != 1;
        public bool HasNextPage => _pageNumber * _pageSize > TotalCount;
    }
}
