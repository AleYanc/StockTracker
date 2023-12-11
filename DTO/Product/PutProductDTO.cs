namespace StockTracker.DTO.Product
{
    public class PutProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double WidthCM { get; set; }
        public double HeightCM { get; set; }
        public double DepthCM { get; set; }
        public double WeightGs { get; set; }
        public bool Active { get; set; } = true;
        public int CategoryId { get; set; }
        public Models.Category Category { get; set; }
    }
}
