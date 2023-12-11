namespace StockTracker.DTO.Sale
{
    public class PostSaleDTO
    {
        public DateTime SaleDate { get; set; }
        public string Status { get; set; }
        public double Total { get; set; }
        public string Currency { get; set; } = "USD";
        public string Products { get; set; }
        public string PaymentMethod { get; set; }
    }
}
