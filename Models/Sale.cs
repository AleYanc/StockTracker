namespace StockTracker.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string Status { get; set; }
        public double Total { get; set; }
        public string Currency { get; set; }
        public string Products { get; set; }
        public string PaymentMethod { get; set; }
    }
}
