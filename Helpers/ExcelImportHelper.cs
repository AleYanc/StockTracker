using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using StockTracker.Models;

namespace StockTracker.Helpers
{
    public class ExcelImportHelper
    {
        public static List<T> Import<T>(string filePath) where T : new()
        {
            XSSFWorkbook workbook;
            using (FileStream stream = new(filePath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(stream);
            }

            ISheet sheet = workbook.GetSheetAt(0);
            IRow rowHeader = sheet.GetRow(0);
            Dictionary<string, int> colIndexList = [];
            foreach (ICell cell in rowHeader.Cells)
            {
                string colName = cell.StringCellValue;
                colIndexList.Add(colName, cell.ColumnIndex);
            }

            List<T> listResult = [];
            int currentRow = 1;

            while (currentRow <= sheet.LastRowNum)
            {
                IRow row = sheet.GetRow(currentRow);
                if (row == null) break;

                T obj = new();

                foreach (var property in typeof(T).GetProperties())
                {
                    if (!colIndexList.ContainsKey(property.Name))
                        throw new Exception($"Column {property.Name} not found.");

                    int colIndex = colIndexList[property.Name];
                    ICell cell = row.GetCell(colIndex);

                    if(cell == null) property.SetValue(obj, null);
                    else if (property.PropertyType == typeof(string))
                    {
                        cell.SetCellType(CellType.String);
                        property.SetValue(obj, cell.StringCellValue);
                    }
                    else if (property.PropertyType == typeof(int))
                    {
                        cell.SetCellType(CellType.Numeric);
                        property.SetValue(obj, Convert.ToInt32(cell.NumericCellValue));
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        cell.SetCellType(CellType.Numeric);
                        property.SetValue(obj, Convert.ToDouble(cell.NumericCellValue));
                    }
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        property.SetValue(obj, cell.DateCellValue);
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        cell.SetCellType(CellType.Boolean);
                        property.SetValue(obj, cell.BooleanCellValue);
                    }
                    else
                    {
                        property.SetValue(obj, Convert.ChangeType(cell.StringCellValue, property.PropertyType));
                    }
                }
                listResult.Add(obj);
                currentRow++;
            }
            return listResult;
        }

        public static ImportHistory BuildNewHistoryRegister(string filePath, string fileName, string importType)
            => new() { FilePath = filePath, FileName = fileName, ImportType = importType };
    }
}
