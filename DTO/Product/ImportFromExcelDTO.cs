using StockTracker.Helpers.Validations;

namespace StockTracker.DTO.Product
{
    public class ImportFromExcelDTO
    {
        [FileTypeValidation(groupTypeFile: GroupTypeFile.Excel)]
        public IFormFile ExcelFile { get; set; }
    }
}
