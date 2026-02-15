using System.Text;
using OfficeOpenXml;
using System.Reflection;

namespace WeatherWebApp.Services
{
    public class ExportService
    {
        public void ExportListToCsvManually<T>(List<T> list, string filePath)
        {
            var csvBuilder = new StringBuilder();
            var properties = typeof(T).GetProperties();

            //headers
            csvBuilder.AppendLine(string.Join("\t", properties.Select(p => p.Name)));

            //data
            foreach (var item in list)
            {
                var row = properties.Select(p => p.GetValue(item, null)?.ToString() ?? "");
                csvBuilder.AppendLine(string.Join("\t", row));
            }

            File.WriteAllText(filePath, csvBuilder.ToString());
        }


        public void ExportListToExcel<T>(List<T> list, string filePath)
        {
            if (list == null || !list.Any())
                throw new ArgumentException("List is empty.");

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            // Header row
            for (int col = 0; col < properties.Length; col++)
            {
                worksheet.Cells[1, col + 1].Value = properties[col].Name;
                worksheet.Cells[1, col + 1].Style.Font.Bold = true;
            }

            // Data rows
            for (int row = 0; row < list.Count; row++)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(list[row]);
                    worksheet.Cells[row + 2, col + 1].Value = value;
                }
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            package.SaveAs(new FileInfo(filePath));
        }

    }
}

