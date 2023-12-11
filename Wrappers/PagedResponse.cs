using Azure;

namespace StockTracker.Wrappers
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Uri FirstPage { get; set; } = new Uri("http://localhost:1234/sample");
        public Uri LastPage { get; set; } = new Uri("http://localhost:1234/sample");
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public Uri NextPage { get; set; } = new Uri("http://localhost:1234/sample");
        public Uri PreviousPage { get; set; } = new Uri("http://localhost:1234/sample");

        public PagedResponse(T data, int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
            this.Message = string.Empty;
            this.Succeeded = true;
            this.Errors = null;
        }
    }
}
