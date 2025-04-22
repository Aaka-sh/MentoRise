//role
//this file defines a helper class for pagination
//it manages how many items to display per page and the current page number
//it also prevents from asking for too many records at once by limiting the maximum page size
namespace API.Helpers;

public class UserParams
{
    private const int MaxPageSize = 50;//maximum number of items per page
    public int PageNumber { get; set; } = 1; //default page number
    private int _pageSize = 10; //default page size
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value; //if the page size exceeds the maximum, set it to the maximum
    }
    public string? Gender { get; set; }
    public string? CurrentUsername { get; set; }

}
