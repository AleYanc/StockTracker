using System.ComponentModel.DataAnnotations.Schema;

namespace StockTracker.Models
{
    public class ProductCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedDate { get; set; }

        [NotMapped]
        public Product Product { get; set; }

    }
}
