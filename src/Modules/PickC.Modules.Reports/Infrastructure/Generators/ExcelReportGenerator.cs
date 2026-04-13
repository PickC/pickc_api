using ClosedXML.Excel;

namespace PickC.Modules.Reports.Infrastructure.Generators;

public class ExcelReportGenerator
{
    public byte[] Generate<T>(string sheetName, IEnumerable<T> data, string[] headers, Func<T, object[]> rowSelector, object[]? footerRow = null)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(sheetName);

        // Header row
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = sheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // Data rows
        int row = 2;
        foreach (var item in data)
        {
            var values = rowSelector(item);
            for (int col = 0; col < values.Length; col++)
                sheet.Cell(row, col + 1).Value = XLCellValue.FromObject(values[col]);
            row++;
        }

        // Footer row (totals)
        if (footerRow != null)
        {
            for (int col = 0; col < footerRow.Length; col++)
            {
                var cell = sheet.Cell(row, col + 1);
                cell.Value = XLCellValue.FromObject(footerRow[col]);
                cell.Style.Font.Bold = true;
            }
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
