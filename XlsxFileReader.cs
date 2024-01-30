namespace TestMDCreator;

using Microsoft.Extensions.Logging;

internal class XlsxFileReader
{
    private readonly ILogger? log;

    public XlsxFileReader(ILogger? log)
    {
        this.log = log;
    }

    public TestModel GetTestModel(string filename)
    {
        TestModel testModel = new TestModel();
        log?.LogInformation($"Reading file '{filename}'");

        using (DocumentFormat.OpenXml.Packaging.SpreadsheetDocument document = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Open(filename, true))
        {
            var workbookPart = document.WorkbookPart;
            if (workbookPart == null)
                throw new NullReferenceException("WorkbookPart is null");

            var sheets = workbookPart.Workbook.Descendants<DocumentFormat.OpenXml.Spreadsheet.Sheet>();
            if (sheets == null)
                throw new NullReferenceException("Sheets is null");

            foreach (var sheet in sheets)
            {
                log?.LogInformation($"Sheet: {sheet.Name}, Id: {sheet.Id}, SheetId: {sheet.SheetId}");
                if (sheet.Name == "Testler")
                {
                    var worksheetPart = (DocumentFormat.OpenXml.Packaging.WorksheetPart)workbookPart.GetPartById(sheet.Id);
                    if (worksheetPart == null)
                        throw new NullReferenceException("WorksheetPart is null");

                    var rows = worksheetPart.Worksheet.Descendants<DocumentFormat.OpenXml.Spreadsheet.Row>();
                    if (rows == null)
                        throw new NullReferenceException("Rows is null");

                    foreach (var row in rows)
                    {
                        log?.LogInformation($"Row: {row.RowIndex}");
                        var cells = row.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>();
                        if (cells == null)
                            throw new NullReferenceException("Cells is null");

                        foreach (var cell in cells)
                        {
                            // Get shared string
                            if (cell.DataType?.Value == DocumentFormat.OpenXml.Spreadsheet.CellValues.SharedString)
                            {
                                var stringTable = workbookPart.GetPartsOfType<DocumentFormat.OpenXml.Packaging.SharedStringTablePart>().FirstOrDefault();
                                if (stringTable == null)
                                    throw new NullReferenceException("StringTable is null");

                                var value = stringTable.SharedStringTable.ElementAt(int.Parse(cell.CellValue?.Text ?? "0")).InnerText;
                                log?.LogInformation($"Cell: {cell.CellReference}, Type: {cell.DataType?.Value}, Value: {value}");
                                if (cell.CellReference?.Value == "A2")
                                    testModel.Name = value;
                                else if (cell.CellReference?.Value == "B2")
                                    testModel.Description = value;
                            }
                        }
                    }
                }

                if (sheet.Name == "Test Adimlari")
                {
                    var worksheetPart = (DocumentFormat.OpenXml.Packaging.WorksheetPart)workbookPart.GetPartById(sheet.Id);
                    if (worksheetPart == null)
                        throw new NullReferenceException("WorksheetPart is null");

                    var rows = worksheetPart.Worksheet.Descendants<DocumentFormat.OpenXml.Spreadsheet.Row>();
                    if (rows == null)
                        throw new NullReferenceException("Rows is null");

                    bool firstRow = true;
                    foreach (var row in rows)
                    {
                        if (firstRow)
                        {
                            firstRow = false;
                            continue;
                        }
                        log?.LogInformation($"Row: {row.RowIndex}");
                        var cells = row.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>();
                        if (cells == null)
                            throw new NullReferenceException("Cells is null");

                        foreach (var cell in cells)
                        {
                            // Get shared string
                            if (cell.DataType?.Value == DocumentFormat.OpenXml.Spreadsheet.CellValues.SharedString)
                            {
                                var stringTable = workbookPart.GetPartsOfType<DocumentFormat.OpenXml.Packaging.SharedStringTablePart>().FirstOrDefault();
                                if (stringTable == null)
                                    throw new NullReferenceException("StringTable is null");

                                var value = stringTable.SharedStringTable.ElementAt(int.Parse(cell.CellValue?.Text ?? "0")).InnerText;
                                log?.LogInformation($"Cell: {cell.CellReference}, Type: {cell.DataType?.Value}, Value: {value}");

                                if (cell.CellReference?.Value?.StartsWith("A") ?? false)
                                {
                                    testModel.Steps ??= new List<string>();
                                    testModel.Steps.Add(value);
                                }
                            }
                        }
                    }
                }
            }
        }

        return testModel;
    }
}
