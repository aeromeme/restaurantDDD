using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class PagedOrderListDto
    {
        public IReadOnlyList<OrderDto> Orders { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage => (PageNumber * PageSize) < TotalCount;
        public bool HasPreviousPage => PageNumber > 1;
    }
}