namespace StockTracker.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string BusinessNumber { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
