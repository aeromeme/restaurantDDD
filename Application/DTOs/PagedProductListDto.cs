using Application.DTOs;

public class PagedProductListDto
{
    public IReadOnlyList<ProductDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage => (PageNumber * PageSize) < TotalCount;
    public bool HasPreviousPage => PageNumber > 1;
}