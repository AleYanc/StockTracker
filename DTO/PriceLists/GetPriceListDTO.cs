using System.ComponentModel.DataAnnotations.Schema;

namespace StockTracker.DTO.PriceLists
{
    public class GetPriceListDTO
    {
        public int Id { get; set; }
        public string PriceType { get; set; }
        public double Price { get; set; }
        public int ProductId { get; set; }
    }
}
