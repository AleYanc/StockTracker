using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StockTracker.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
