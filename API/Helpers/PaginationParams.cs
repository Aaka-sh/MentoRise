using System;

namespace API.Helpers;

public class PaginationParams
{
    private const int MaxPageSize = 50;//maximum number of items per page
    public int PageNumber { get; set; } = 1; //default page number
    private int _pageSize = 10; //default page size
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value; //if the page size exceeds the maximum, set it to the maximum
    }
}
