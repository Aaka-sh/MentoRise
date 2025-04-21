// role
// this file adds a custom header to an API response
// the custom header includes pagination information about the data being returned: current page, total pages, number of items on current page and total number of items

using System.Text.Json;
using API.Helpers;

namespace API.Extensions;

//extension methods are defined as static methods in a static class
public static class HttpExtensions
{
    public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
    {
        var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize,
            data.TotalCount, data.TotalPages);

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }
}
