//role
//this file defines a helper class for pagination
//it manages how many items to display per page and the current page number
//it also prevents from asking for too many records at once by limiting the maximum page size
namespace API.Helpers;

public class UserParams : PaginationParams
{
    public string? Gender { get; set; }
    public string? CurrentUsername { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;
    public string OrderBy { get; set; } = "lastActive"; //default order by last active

}
