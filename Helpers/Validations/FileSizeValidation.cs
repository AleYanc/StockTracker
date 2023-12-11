using System.ComponentModel.DataAnnotations;

namespace StockTracker.Helpers.Validations
{
    public class FileSizeValidation : ValidationAttribute
    {
        private readonly int maxSizeMb;

        public FileSizeValidation(int MaxSizeMb)
        {
            maxSizeMb = MaxSizeMb;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            IFormFile formFile = value as IFormFile;

            if (formFile == null) return ValidationResult.Success;

            if (formFile.Length > maxSizeMb * 1024 * 1024)
            {
                return new ValidationResult($"Max file size is {maxSizeMb}");
            }

            return ValidationResult.Success;
        }
    }
}
