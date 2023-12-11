using StockTracker.Helpers.Validations;
using StockTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace StockTracker.DTO.Product
{
    public class PostProductDTO
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [FileSizeValidation(4)]
        [FileTypeValidation(groupTypeFile: GroupTypeFile.Image)]
        public List<IFormFile> Images { get; set; }
        [Required]
        public double WidthCM { get; set; }
        public double HeightCM { get; set; }
        public double DepthCM { get; set; }
        public double WeightGs { get; set; }
        public bool Active { get; set; } = true;
        public int CategoryId { get; set; }
    }
}
