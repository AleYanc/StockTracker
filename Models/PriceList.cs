namespace StockTracker.Models
{
    public class PriceList
    {
        public int Id { get; set; }
        public string PriceType { get; set; }
        public double Price { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
