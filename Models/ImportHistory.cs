namespace StockTracker.Models
{
    public class ImportHistory
    {
        public int Id { get; set; }
        public string ImportType { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime ImportDate { get; set; }
    }
}
