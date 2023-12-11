using StockTracker.DTO;
using StockTracker.Services;
using StockTracker.Wrappers;

namespace StockTracker.Helpers
{
    public class PaginationHelper
    {
        public static PagedResponse<List<T>> CreatePagedResponse<T>(List<T> pagedData, PaginationDTO pagination, 
            int totalRecords, IUriService uriService, string route)
        {
            PagedResponse<List<T>> response = new PagedResponse<List<T>>(pagedData, pagination.PageNumber, pagination.PageSize);
            double totalPages = ((double)totalRecords / (double)pagination.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            response.NextPage =
                pagination.PageNumber >= 1 && pagination.PageNumber < roundedTotalPages
                ? uriService.GetPageUri(new PaginationDTO(pagination.PageNumber + 1, pagination.PageSize), route)
                : null;
            response.PreviousPage =
                pagination.PageNumber - 1 >= 1 && pagination.PageNumber <= roundedTotalPages
                ? uriService.GetPageUri(new PaginationDTO(pagination.PageNumber - 1, pagination.PageSize), route)
                : null;
            response.FirstPage = uriService.GetPageUri(new PaginationDTO(1, pagination.PageSize), route);
            response.LastPage = uriService.GetPageUri(new PaginationDTO(roundedTotalPages, pagination.PageSize), route);
            response.TotalPages = roundedTotalPages;
            response.TotalRecords = totalRecords;
            return response;
        }
    }
}
