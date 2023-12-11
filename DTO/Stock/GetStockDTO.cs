using System.ComponentModel.DataAnnotations.Schema;

namespace StockTracker.DTO.Stock
{
    public class GetStockDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
