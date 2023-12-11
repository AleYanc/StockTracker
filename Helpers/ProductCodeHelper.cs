using Microsoft.EntityFrameworkCore;
using StockTracker.Database;
using StockTracker.Models;

namespace StockTracker.Helpers
{
    public class ProductCodeHelper
    {
        public async Task<string> BuildCode(DatabaseConnection context)
        {
            ProductCode product = await context.ProductCodes
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if(product == null) { return Lpad(1); }

            int latestCode = int.Parse(product.Code);
            int newProductCode = latestCode + 1;

            string productCode = Lpad(newProductCode);

            return productCode;
        }

        private static string Lpad(int numberToPad)
        {
            int desiredLength = 10;
            int numberLength = numberToPad.ToString().Length;

            string zeros = new string('0', desiredLength - numberLength);

            string padNumber = string.Format("{0}{1}", zeros, numberToPad.ToString());

            return padNumber;
        }
    }
}
