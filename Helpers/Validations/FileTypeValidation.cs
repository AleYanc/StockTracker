using System.ComponentModel.DataAnnotations;

namespace StockTracker.Helpers.Validations
{
    public class FileTypeValidation : ValidationAttribute
    {
        private readonly string[] validTypes;

        public FileTypeValidation(string[] validTypes)
        {
            this.validTypes = validTypes;
        }

        public FileTypeValidation(GroupTypeFile groupTypeFile)
        {
            switch (groupTypeFile)
            {
                case GroupTypeFile.Image:
                    validTypes = new string[] { "image/jpeg", "image/png", "image/gif" };
                    break;
                case GroupTypeFile.Excel:
                    validTypes = new string[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };
                    break;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            IFormFile formFile = value as IFormFile;

            if (formFile == null) return ValidationResult.Success;

            if (!validTypes.Contains(formFile.ContentType))
            {
                return new ValidationResult($"The only accepted file types are ' {string.Join(", ", validTypes)} '");
            }

            return ValidationResult.Success;
        }
    }
}
