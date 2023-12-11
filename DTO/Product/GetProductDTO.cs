using StockTracker.Models;

namespace StockTracker.DTO.Product
{
    public class GetProductDTO
    {
        public int Id { get; set; }
        public SimpleCodeDTO ProductCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagesUrl { get; set; }
        public double WidthCM { get; set; }
        public double HeightCM { get; set; }
        public double DepthCM { get; set; }
        public double WeightGs { get; set; }
        public bool Active { get; set; }
        public SimpleStockDTO Stock { get; set; }
        public SimpleCategoryDTO Category { get; set; } = null!;
    }
}
