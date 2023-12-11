using System.ComponentModel.DataAnnotations.Schema;

namespace StockTracker.DTO.Stock
{
    public class PostStockDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
