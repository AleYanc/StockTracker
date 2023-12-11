using Microsoft.AspNetCore.WebUtilities;
using StockTracker.DTO;

namespace StockTracker.Services
{
    public class UriService(string baseUri) : IUriService
    {
        private readonly string _baseUri = baseUri;

        public Uri GetPageUri(PaginationDTO pagination, string route)
        {
            Uri _endpointUri = new Uri(string.Concat(_baseUri, route));
            string modifiedUri = QueryHelpers.AddQueryString(_endpointUri.ToString(),
                "pageNumber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pagination.PageSize.ToString());
            return new Uri(modifiedUri);
        }
    }
}
