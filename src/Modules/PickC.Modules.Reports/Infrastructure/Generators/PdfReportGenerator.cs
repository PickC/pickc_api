using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PickC.Modules.Reports.Infrastructure.Generators;

public class PdfReportGenerator
{
    public byte[] Generate<T>(string title, string subtitle, IEnumerable<T> data, string[] headers, Func<T, string[]> rowSelector, string[]? footerRow = null)
    {
        var rows = data.ToList();
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Column(col =>
                {
                    col.Item().Text(title).Bold().FontSize(16).FontColor(Colors.Blue.Darken2);
                    col.Item().Text(subtitle).FontSize(10).FontColor(Colors.Grey.Darken1);
                    col.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                });

                page.Content().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        for (int i = 0; i < headers.Length; i++)
                            cols.RelativeColumn();
                    });

                    // Header row
                    table.Header(header =>
                    {
                        foreach (var h in headers)
                        {
                            header.Cell().Background(Colors.Blue.Darken2).Padding(4)
                                .Text(h).Bold().FontColor(Colors.White).FontSize(8);
                        }
                    });

                    // Data rows
                    bool alternate = false;
                    foreach (var item in rows)
                    {
                        var values = rowSelector(item);
                        var bg = alternate ? Colors.Grey.Lighten3 : Colors.White;
                        foreach (var v in values)
                            table.Cell().Background(bg).Padding(3).Text(v).FontSize(8);
                        alternate = !alternate;
                    }

                    // Footer totals
                    if (footerRow != null)
                    {
                        foreach (var v in footerRow)
                            table.Cell().Background(Colors.Grey.Lighten1).Padding(3)
                                .Text(v).Bold().FontSize(8);
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ").FontSize(8);
                    x.CurrentPageNumber().FontSize(8);
                    x.Span(" of ").FontSize(8);
                    x.TotalPages().FontSize(8);
                });
            });
        });

        return document.GeneratePdf();
    }
}
