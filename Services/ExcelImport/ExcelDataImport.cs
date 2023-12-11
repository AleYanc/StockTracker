using System.Data;
using System.Data.OleDb;

namespace StockTracker.Services.ExcelImport
{
    public class ExcelDataImport
    {
        public DataTable ImportExcelData(string fileUrl, string query)
        {
            // WORK IN PROGRESS
            string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0';", fileUrl); 
            //@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileUrl + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';";
            #pragma warning disable CA1416 // Use 'new(...)'
            OleDbConnection connection = new OleDbConnection(connectionString);
            connection.Open();
            OleDbCommand command = new OleDbCommand(query, connection);

            DataTable Data = new DataTable();
            OleDbDataAdapter adapter = new OleDbDataAdapter(command);
            adapter.Fill(Data);
            connection.Close();

            // Check for empty rows and delete them
            for(int i = Data.Rows.Count; i >= 0; i--) 
            {
                if (Data.Rows[i][0].ToString() == String.Empty)
                {
                    Data.Rows.RemoveAt(i);
                }
            }

            return Data;
        }
    }
}
