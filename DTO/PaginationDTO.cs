namespace StockTracker.DTO
{
    public class PaginationDTO
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PaginationDTO()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public PaginationDTO(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}
