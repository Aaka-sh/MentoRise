//role of thie file
//this helper class stores metadata about paginated results: page number, number of items on the page, total number of items, total number of pages
//it will send pagination information in the response header to the client

namespace API.Helpers;

//in this class, we use primary contructor syntax to define the properties of the class
public class PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
{
    public int CurrentPage { get; set; } = currentPage;
    public int ItemsPerPage { get; set; } = itemsPerPage;
    public int TotalItems { get; set; } = totalItems;
    public int TotalPages { get; set; } = totalPages;
}
