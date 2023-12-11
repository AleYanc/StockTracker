namespace StockTracker.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagesUrl { get; set; }
        public double WidthCM { get; set; }
        public double HeightCM { get; set; }
        public double DepthCM { get; set; }
        public double WeightGs { get; set; }
        public double Cost { get; set; }
        public bool Active { get; set; }
        public Stock? Stock { get; set; }
        public ProductCode? ProductCode { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public Supplier? Supplier { get; set; }
        public ICollection<PriceList> Prices { get; set; } = new List<PriceList>();
    }
}
