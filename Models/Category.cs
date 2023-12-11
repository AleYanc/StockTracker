using System.ComponentModel.DataAnnotations.Schema;

namespace StockTracker.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public ICollection<Product> Products { get; set; }
    }
}
