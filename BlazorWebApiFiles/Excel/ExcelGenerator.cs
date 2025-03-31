using Google.Protobuf.WellKnownTypes;
using Syncfusion.XlsIO;

namespace BlazorIdentity.Files.Excel
{
    public class ExcelGenerator
    {
        public byte[] GenerateExcelFromTemplate<T>(string templatePath, List<T> data, int startRow, List<ExcelCellData>? cellDataList)
        {
            // Load the template Excel file
            using ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            // Open the template workbook
            FileStream templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = application.Workbooks.Open(templateStream);

            // Get the first worksheet in the template
            IWorksheet worksheet = workbook.Worksheets[0];

            // Start populating data from the second row (assuming the first row is the header)
            var mappings = ExcelMappingHelper.GetColumnMappings<T>(worksheet, startRow);

            // Fill each cell in the list
            if (cellDataList != null && cellDataList.Any())
            {
                foreach (var cellData in cellDataList)
                {
                    worksheet.Range[cellData.CellAddress].CellStyle = worksheet.Range[cellData.CellAddress].CellStyle;
                    worksheet.Range[cellData.CellAddress].Text = cellData.Value;
                }
            }

            worksheet.DeleteRow(startRow);

            // Start populating data from the specified start row
            foreach (var item in data)
            {
                worksheet.InsertRow(startRow, 1);
                foreach (var mapping in mappings)
                {
                    var property = typeof(T).GetProperty(mapping.PropertyName);
                    var value = property?.GetValue(item);
                    worksheet[startRow, mapping.ColumnIndex].CellStyle = worksheet[startRow - 1, mapping.ColumnIndex].CellStyle;

                    worksheet[startRow, mapping.ColumnIndex].Value = value?.ToString();
                }
            }

            // Save the changes to a new memory stream
            using MemoryStream memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);

            // Return the byte array of the generated Excel file
            return memoryStream.ToArray();
        }
    }

    public class ExcelCellData
    {
        public string CellAddress { get; set; }
        public string Value { get; set; }
    }

    public class ExcelColumnMapping
    {
        public string PropertyName { get; set; }
        public int ColumnIndex { get; set; }
    }

    public static class ExcelMappingHelper
    {
        public static List<ExcelColumnMapping> GetColumnMappings<T>(IWorksheet worksheet, int headerRowIndex = 1)
        {
            var mappings = new List<ExcelColumnMapping>();
            var properties = typeof(T).GetProperties();

            for (int col = 1; col <= worksheet.Columns.Length; col++)
            {
                var headerValue = worksheet[headerRowIndex, col].Value?.ToString();
                if (!string.IsNullOrEmpty(headerValue))
                {
                    var property = properties.FirstOrDefault(p => p.Name.Equals(headerValue, StringComparison.OrdinalIgnoreCase));
                    if (property != null)
                    {
                        mappings.Add(new ExcelColumnMapping
                        {
                            PropertyName = property.Name,
                            ColumnIndex = col
                        });
                    }
                }
            }

            return mappings;
        }
    }
}
