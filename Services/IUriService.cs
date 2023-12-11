using StockTracker.DTO;

namespace StockTracker.Services
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationDTO pagination, string route);
    }
}
